#ifndef __MESH_H__
#define __MESH_H__

#include "SerializationManager.h"
#include "Point3f.h"
#include "String.h"
#include "AABBox.h"
#include "Factory.h"
#include "Renderable.h"
#include "Vector.h"

class Mesh;

class ColorChannel
{
public:

	ColorChannel(int size): m_colors(size) {}

	ColorChannel(const SerializationManager &sm): m_colors(sm)
	{	
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(m_colors);		
	}

	Vector<int> m_colors;
};

class TexChannel
{
public:

	TexChannel(int size): m_texCoords(size, Point3f(0.0f, 0.0f, 0.0f)) {}

	TexChannel(const SerializationManager &sm): m_texCoords(sm)
	{	
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(m_texCoords);		
	}

	Vector<Point3f> m_texCoords;
};

class Material
{
public:

	Material(const char *szName): m_desc(szName) {}

	Material(const SerializationManager &sm): m_desc(sm)
	{
		sm.RelocatePointer(m_mesh);
	}

	inline void CollectPtrs(SerializationManager &sm) const;

	int m_iBaseIndex;
	int m_iStartIndex;
	int m_iEndIndex;

	Mesh *m_mesh;

	String m_desc;
};


	
class Mesh: public Renderable
{
public:

	CLASSID(602)

	Mesh(bool): m_mats(0, Material("test")), m_primitiveType(0), m_vertices(0, Point3f(0.0f, 0.0f, 0.0f)), 
		m_normals(0, Point3f(0.0f, 0.0f, 0.0f)), m_colors(0, ColorChannel(0)), m_texCoords(0, TexChannel(0)),
		m_tangents(0, Point3f(0.0f, 0.0f, 0.0f)), 
		m_binormals(0, Point3f(0.0f, 0.0f, 0.0f)), m_indices(0), 
		m_bShowNormals(false), m_bShowTangentSpaceMatrix(false), 
		m_bbox(Point3f(0.0f, 0.0f, 0.0f), Point3f(0.0f, 0.0f, 0.0f)), m_bAABBoxDirty(true)
	{
	}

	void Render() const {}

	Mesh(const SerializationManager &sm): m_mats(sm), m_vertices(sm), m_normals(sm), m_colors(sm),
		m_texCoords(sm), m_tangents(sm), m_binormals(sm), m_indices(sm), m_bbox(sm)
	{
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(m_mats);
		sm.CollectPtrs(m_vertices);
		sm.CollectPtrs(m_normals);
		sm.CollectPtrs(m_colors);
		sm.CollectPtrs(m_texCoords);
		sm.CollectPtrs(m_tangents);
		sm.CollectPtrs(m_binormals);
		sm.CollectPtrs(m_indices);
		sm.CollectPtrs(m_bbox);
	}

	////////

	Vector<Material> m_mats;	

	int m_primitiveType;

	Vector<Point3f> m_vertices;
	Vector<Point3f> m_normals;
	Vector<ColorChannel> m_colors;
	Vector<TexChannel> m_texCoords;
	Vector<Point3f> m_tangents;
	Vector<Point3f> m_binormals;

	Vector<short> m_indices;

	bool m_bShowNormals;
	bool m_bShowTangentSpaceMatrix;

	AABBox m_bbox;
	bool m_bAABBoxDirty;
};

/////////////////////////////////////////////////////////////////////

void Material::CollectPtrs(SerializationManager &sm) const
{
	sm.CollectPtrs(m_mesh);
	sm.CollectPtrs(m_desc);
}


#endif