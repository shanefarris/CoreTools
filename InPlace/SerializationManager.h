#ifndef __SERIALIZATIONMANAGER_H__
#define __SERIALIZATIONMANAGER_H__

#include <windows.h>

#include "Factory.h"
#include "CompileTools.h"
#include "Resource.h"

#include <vector>
#include <list>
#include <stack>
#include <set>
#include <algorithm>
#include <typeinfo>

// SerializationManager. Manager in charge of loading/saving objects to/from a file

class SerializationManager
{
public:

	SerializationManager(Factory &factory): m_factory(factory), m_nextAllocatedNodeID(0)
	{
	}

	Factory &GetFactory() const
	{
		return m_factory;
	}

	// Save functions

	template<class T> void Save(const char *szFileName, T *const &root)
	{	
		Save(szFileName, *root);		
	}
	
	template<class T> void Save(const char *szFileName, T const &root)
	{
		assert(m_allocatedNodes.empty());
		assert(m_storedPointers.empty());
		assert(nodeStack.empty());

		// Create the node tree collecting pointers

		Node *rootNode = AllocNode();

		rootNode->m_ptr = 0;		

		nodeStack.push(rootNode);
			CollectPtrs(&root);
		nodeStack.pop();

		assert(nodeStack.empty());

		// We remove here the backpointer to the rootNode (which is going to be released)
		assert(rootNode->m_pointers.size() == 1);		
		std::list<Node::NodeRef>::iterator firstPointerIt = rootNode->m_pointers.begin();		
		assert(*(firstPointerIt->node->m_backPointers.begin()) == &(*firstPointerIt));
		firstPointerIt->node->m_backPointers.erase(firstPointerIt->node->m_backPointers.begin());
		rootNode->m_pointers.clear();
				
		ReleaseNode(rootNode);		
			
		// Save the collected nodes

		size_t size = 0;
		size_t iNumPointers = 0;

		std::set<Node *, NodeCmp>::iterator it;

		// 1. Calculate the total filesize

		for(it = m_allocatedNodes.begin(); it != m_allocatedNodes.end(); ++it)
		{
			(*it)->m_filePos = size;			
			size += (*it)->m_data.size();
			iNumPointers += (*it)->m_pointers.size();
		}

		printf("-SAVE- %s: %d nodes in %d bytes. %d pointers\n", szFileName, m_allocatedNodes.size(), size, iNumPointers);			

		// The first node is marked as created (it is firstly created with a new)		

		(*m_allocatedNodes.begin())->m_bCreated = true;

		// 2. Convert pointers to offsets and write to disk

		FILE *fp = fopen(szFileName, "wb");

		for(it = m_allocatedNodes.begin(); it != m_allocatedNodes.end(); ++it)
		{
			std::list<Node::NodeRef *>::iterator backPointersIt;

			// This assert may be too restrictive. Imagine this scenario:
			//
			// class B { float val; };
			// class A { B b; };
			//			
			// A a;
			//
			// float *ptr0 = &a.b.val;
			// B *ptr1 = &a.b;
			// A *ptr2 = &a;
			//
			// In this sample, the pointers ptr0, ptr1 and ptr2 point to the same region.
			// The dominant pointer (the one that should do the placement new) is ptr2 here (pointer to A). The dominant
			//	pointer is the biggest one. In some situations you cannot autodetect the dominant pointer (pointers
			//	with same size and different type).
			// In this sample, we do not detect dominant pointers. Instead, we assume all the pointers to a node
			//	are of the same type. It this condition if not true, and assert will be fired

			for(backPointersIt = (*it)->m_backPointers.begin(); backPointersIt != (*it)->m_backPointers.end(); ++backPointersIt)
			{
				Node::NodeRef *nodeRef = *backPointersIt;

				if(nodeRef->delta == 0)
				{
					assert(*(nodeRef->typeInfo) == *((*it)->m_typeInfo));
				}
								
			}

			std::list<Node::NodeRef>::iterator pointersIt;

			for(pointersIt = (*it)->m_pointers.begin(); pointersIt != (*it)->m_pointers.end(); ++pointersIt)
			{
				size_t *ptr = (size_t *)&((*it)->m_data[pointersIt->offset]);								

				// Pointers inside a node (delta != 0) won't never be created with new
				// We add 4 to the offsets so the null pointers is still a 0
				// Pointers to blocks that needs to be created with new (the first node referencing it) are
				//	marked with a 1 in the highgbit

				if(pointersIt->delta != 0)
				{
					*ptr = pointersIt->node->m_filePos + pointersIt->delta + 4;
				}
				else
				{					
					// If the node was previously accessed mask it to not create it again
					if(pointersIt->node->m_bCreated)
					{
						*ptr = pointersIt->node->m_filePos + pointersIt->delta + 4;
					}
					else
					{
						*ptr = (pointersIt->node->m_filePos + pointersIt->delta + 4) | 0x80000000;
						pointersIt->node->m_bCreated = true;
					}						
				}							
			}

			fwrite(&(*it)->m_data[0], (*it)->m_data.size(), 1, fp);
		}

		fclose(fp);		

		// Free memory

		for(it = m_allocatedNodes.begin(); it != m_allocatedNodes.end(); ++it)
		{
			assert((*it)->m_bCreated);
			(*it)->Release();			
		}

		m_allocatedNodes.clear();
		m_storedPointers.clear();
	}

	// Load functions

	template<class T> void Load(const char *szFileName, Resource<T> &resource)
	{
		LARGE_INTEGER t0, t1, t2;
		QueryPerformanceCounter(&t0);
		
		FILE *fp = fopen(szFileName, "rb");
		fseek(fp, 0, SEEK_END);
		size_t size = ftell(fp);
		fseek(fp, 0, SEEK_SET);

		resource.m_data = new char[size];
		fread(resource.m_data, size, 1, fp);
		fclose(fp);
				
		m_baseMemory = resource.m_data - 4;

		// First node: offset 0(+4), must be created with new(&0x80000000)
		T *root = (T*)((char *)4 + 0x80000000);

		QueryPerformanceCounter(&t1);
				   				
		RelocatePointer(root);	

		QueryPerformanceCounter(&t2);

		float part0 = 100.0f * (t1.QuadPart - t0.QuadPart ) / (t2.QuadPart - t0.QuadPart);
		float part1 = 100.0f * (t2.QuadPart - t1.QuadPart ) / (t2.QuadPart - t0.QuadPart);

		printf("-LOAD- %s: I/O: %4.2f%% Relocate: %4.2f%%\n", szFileName, part0, part1);			
	}

	// CollectPtrs functions. Used when saving objects to detect all the pointers inside the object tree
		
	template<class T> void CollectPtrs(T const &ptr)
	{
		ptr.CollectPtrs(*this);
	}	

	// Basic types
	
	void CollectPtrs(char const &) {}
	void CollectPtrs(signed char const &) {}
	void CollectPtrs(unsigned char const &) {}

	void CollectPtrs(short const &) {}	
	void CollectPtrs(unsigned short const &) {}

	void CollectPtrs(int const &) {}	
	void CollectPtrs(unsigned int const &) {}

	void CollectPtrs(long const &) {}	
	void CollectPtrs(unsigned long const &) {}

	void CollectPtrs(float const &) {}	
	void CollectPtrs(double const &) {}

	template<class T> void CollectPtrs(T * const &ptr, size_t arrayLength = 1)
	{		
		Node *root = nodeStack.top();

		if(ptr != 0)
		{
			size_t offset;
			Node *node = 0;
			
			// Check if this is the first time we visit this node
			if(!FindPointer(ptr, offset, node))
			{
				node = AllocNode();

				Node::NodeRef ptrData;
				ptrData.offset = size_t(&ptr) - size_t(root->m_ptr);
				ptrData.node = node;
				ptrData.delta = 0;				
				ptrData.typeInfo = SaveObjectHelper<T, SUPERSUBCLASS(BaseObject, T)>::GetTypeInfo();

				// Add a forward pointer to the root node
				root->m_pointers.push_back(ptrData);

				// Add a backward pointer to the current node
				node->m_backPointers.push_back(&root->m_pointers.back());

				node->m_ptr = ptr;
				node->m_typeInfo = ptrData.typeInfo;

				// Arrays are never treat polymorphically				
				if(arrayLength > 1)
				{
					SaveObjectHelper<T, false>::SaveObject(*this, ptr, node, arrayLength);
				}
				else
				{
					SaveObjectHelper<T, SUPERSUBCLASS(BaseObject, T)>::SaveObject(*this, ptr, node, arrayLength);
				}
								
				node->AddReference();
				nodeStack.push(node);

				// Continue down this node

				for(int i = 0; i < (int)arrayLength; i++)
				{					
					CollectPtrs(ptr[i]);
				}
				
				nodeStack.pop();		
				node->Release();
			}
			else
			{
				Node::NodeRef ptrData;
				ptrData.offset = size_t(&ptr) - size_t(root->m_ptr);
				ptrData.node = node;
				ptrData.delta = offset;				
				ptrData.typeInfo = SaveObjectHelper<T, SUPERSUBCLASS(BaseObject, T)>::GetTypeInfo();

				root->m_pointers.push_back(ptrData);
				node->m_backPointers.push_back(&root->m_pointers.back());
			}
		}
	}
	
	template<class T> void RelocatePointer(T *&ptr) const
	{
		InternalRelocatePointer<true>(ptr);
	}

	template<class T> void RelocatePointer(T *&ptr, size_t arrayLength) const
	{
		InternalRelocatePointer<true>(ptr, arrayLength);
	}

private:
	
	// Node holds information about a contiguous block of memory

	class Node
	{
	public:

		Node(): m_nodeID(0), m_refCount(1), m_filePos(0), m_bCreated(false), m_ptr(0), m_typeInfo(0)
		{
		}

		~Node()		
		{
			std::list<Node::NodeRef *>::iterator backIt;

			for(backIt = m_backPointers.begin(); backIt != m_backPointers.end(); ++backIt)
			{
				(*backIt)->node = 0;							
			}

			// Disconnect from the backPointers of the referenced nodes

			std::list<Node::NodeRef>::iterator ptrIt;

			for(ptrIt = m_pointers.begin(); ptrIt != m_pointers.end(); ++ptrIt)
			{
				if(ptrIt->node)
				{
					std::list<Node::NodeRef *>::iterator findIt = 
						std::find(ptrIt->node->m_backPointers.begin(), ptrIt->node->m_backPointers.end(), &(*ptrIt));
					assert(findIt != ptrIt->node->m_backPointers.end());

					ptrIt->node->m_backPointers.erase(findIt);			
				}
			}
		}

		size_t AddReference()
		{
			return ++m_refCount;
		}

		size_t Release()
		{		
			if(--m_refCount == 0)
			{
				delete this;
				return 0;
			}
			
			return m_refCount;
		}

		int m_nodeID;

		// Reference Counter
		size_t m_refCount;

		// Position where this block is stored in disk
		size_t m_filePos;

		// The first reference to this node in the file is marked with a flag to be created (new). The rests of
		//	the following references do not create the node
		bool m_bCreated;

		// Pointer to the contiguous block of memory
		const void *m_ptr;

		// type_info of m_ptr
		const std::type_info *m_typeInfo;

		// A copy of m_ptr with the pointers and vtables properly adjusted to be saved
		std::vector<char> m_data;

		// A reference to other node
		struct NodeRef
		{
			// Offset relative to Node::m_ptr where the reference (pointer) is stored
			size_t offset;

			// Node referenced
			Node *node;

			// Offset inside node where NodeRef points out
			size_t delta;
			
			// type_info of the pointed data
			const std::type_info *typeInfo;
		};

		// List of pointers to others nodes
		std::list<NodeRef> m_pointers;

		// List of pointers pointing to this node
		std::list<NodeRef *> m_backPointers;
	};
	
	struct MemoryBlock
	{
		MemoryBlock(const void *pos0, const void *pos1): start(pos0), end(pos1) {}
		MemoryBlock(const void *pos): start(pos), end(pos) {}

		bool operator<(const MemoryBlock &memoryBlock) const
		{
			return end < memoryBlock.end;
		}

		const void *start; 
		const void *end;
	};

	// Helper class for saving an object into a buffer

	template<class T, bool IsPolymorphic>
	class SaveObjectHelper
	{
	public:

		// Non-polymoprhic instances are simply memcopied

		static void SaveObject(SerializationManager &sm, T * const &ptr, Node *node, size_t arrayLength)
		{
			node->m_data.resize(sizeof(T) * arrayLength);
			memcpy(&node->m_data[0], ptr, sizeof(T) * arrayLength);
			
			sm.InsertMemoryBlock(MemoryBlock(ptr, (char *)ptr + sizeof(T) * arrayLength), node);
		}

		static size_t GetSize(T * const &ptr)
		{
			return sizeof(T);
		}

		static const type_info *GetTypeInfo()
		{
			return &typeid(T);
		}
	};

	template<class T> 
	class SaveObjectHelper<T, true>
	{
	public:

		// Polymorphic instances need to get the size through the virtual function GetSize().
		//	They write the ClassID in the vtable position

		static void SaveObject(SerializationManager &sm, T * const &ptr, Node *node, size_t arrayLength)
		{			
			// To avoid the warning
			arrayLength;

			// Arrays are never treated polymorphically			
			assert(arrayLength == 1);

			node->m_data.resize(ptr->GetSize());
			memcpy(&node->m_data[0], ptr, ptr->GetSize());

			// Write ClassID in the vtable position
			*((int *)&node->m_data[0]) = ptr->GetClassID();

			sm.InsertMemoryBlock(MemoryBlock(ptr, (char *)(ptr) + ptr->GetSize()), node);
		}

		static const type_info *GetTypeInfo()
		{
			return &typeid(BaseObject);
		}	
	};

	// Helper class for creating an object in a buffer (new inplace or factory creation)

	template<class T, bool IsPolymorphic>
	class LoadInPlaceHelper
	{
	public:

		// Non-polymoprhic instances

		static void LoadInPlace(const SerializationManager &sm, const T &ptr)
		{
			new((void *)&ptr) T(sm);
		}
	};

	template<class T>
	class LoadInPlaceHelper<T, true>
	{
	public:

		// Polymoprhic instances

		static void LoadInPlace(const SerializationManager &sm, const T &ptr)
		{
			sm.m_factory.CreateObject(*((int *)(&ptr)), (void*)(&ptr), sm);
		}
	};	

	// InternalStorePointer functions

	template<bool IsPolymorphic, class T> void InternalRelocatePointer(const T &ptr) const	
	{
		LoadInPlaceHelper<T, IsPolymorphic>::LoadInPlace(*this, ptr);
	}

	// Basic types

	template<bool IsPolymorphic> void InternalRelocatePointer(const char &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const signed char &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const unsigned char &) const {}

	template<bool IsPolymorphic> void InternalRelocatePointer(const short &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const unsigned short &) const {}

	template<bool IsPolymorphic> void InternalRelocatePointer(const int &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const unsigned int &) const {}

	template<bool IsPolymorphic> void InternalRelocatePointer(const long &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const unsigned long &) const {}

	template<bool IsPolymorphic> void InternalRelocatePointer(const float &) const {}
	template<bool IsPolymorphic> void InternalRelocatePointer(const double &) const {}

	template<bool IsPolymorphic, class T>  void InternalRelocatePointer(T *&ptr) const
	{
		if(ptr)
		{
			int delta = *((int *)&ptr);

			if(delta & 0x80000000)
			{
				ptr = (T *)(m_baseMemory + (delta & (~0x80000000)));
				InternalRelocatePointer<SUPERSUBCLASS(BaseObject, T)>(*ptr);					

			}
			else
			{
				ptr = (T *)(m_baseMemory + delta);
			}		
		}
	}

	template<bool IsPolymorphic, class T> void InternalRelocatePointer(T *&ptr, size_t arrayLength) const
	{
		if(ptr)
		{
			int delta = *((int *)&ptr);

			if(delta & 0x80000000)
			{
				ptr = (T *)(m_baseMemory + (delta & (~0x80000000)));

				for(int i = 0; i < (int)arrayLength; i++)
				{			
					// Arrays are never treated polymorphically
					InternalRelocatePointer<false>(ptr[i]);					
				}
			}
			else
			{
				ptr = (T *)(m_baseMemory + delta);
			}		
		}
	}	

	// Check if a pointer is already stored in the node tree

	bool FindPointer(const void *ptr, size_t &offset, Node *&node)
	{
		std::map<MemoryBlock, Node *>::iterator upperBound = m_storedPointers.upper_bound(ptr);

		if(upperBound != m_storedPointers.end() && upperBound->first.start <= ptr)
		{
			offset = (char *)ptr - (char *)upperBound->first.start;
			node = upperBound->second;
			return true;
		}
		else
		{
			return false;
		}
	}

	// When a new node is inserted into the tree we look for previous nodes whose memory is contained in this
	//	new node. If we find one, we remove that node (adjusting all the references) because it is contained
	//	in this one
	
	void InsertMemoryBlock(const MemoryBlock &memoryBlock, Node *node)
	{
		std::map<MemoryBlock, Node *>::iterator it0 = m_storedPointers.upper_bound(memoryBlock.start);
		std::map<MemoryBlock, Node *>::iterator it1 = m_storedPointers.upper_bound(memoryBlock.end);		
	
		while(it0 != it1)
		{
			Node *nodeToBeDeleted = it0->second;

			size_t offset = size_t(nodeToBeDeleted->m_ptr) - size_t(node->m_ptr);				

			// Adjusting back pointers

			std::list<Node::NodeRef *>::iterator backIt;

			for(backIt = nodeToBeDeleted->m_backPointers.begin(); backIt != nodeToBeDeleted->m_backPointers.end(); ++backIt)
			{
				(*backIt)->node = node;
				(*backIt)->delta += offset;

				node->m_backPointers.push_back(*backIt);
			}

			nodeToBeDeleted->m_backPointers.clear();

			// Remove the node
			ReleaseNode(nodeToBeDeleted);

			// Next node
			it0 = m_storedPointers.erase(it0);						
		}
		
		m_storedPointers[memoryBlock] = node;
	}

	Node *AllocNode()
	{
		Node *node = new Node;
		node->m_nodeID = m_nextAllocatedNodeID++;
		m_allocatedNodes.insert(node);
		return node;
	}

	void ReleaseNode(Node *node)
	{		
		assert(m_allocatedNodes.find(node) != m_allocatedNodes.end());
		m_allocatedNodes.erase(node);

		node->Release();
	}

	///////////////////////////////////////////////

private:

	SerializationManager &operator=(const SerializationManager &sm);

	Factory &m_factory;

	// Save data

	std::stack<Node *> nodeStack;
	std::map<MemoryBlock, Node *> m_storedPointers;

	class NodeCmp
	{
	public:

		bool operator()(const Node *node0, const Node *node1) const
		{
			return node0->m_nodeID < node1->m_nodeID;			
		}
	};

	int m_nextAllocatedNodeID;
	std::set<Node *, NodeCmp> m_allocatedNodes;

	// Load data

	char *m_baseMemory;	

};

#endif