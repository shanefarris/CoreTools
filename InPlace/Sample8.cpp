#include "Sample8.h"
#include "Mesh.h"

void Sample8(SerializationManager &manager)
{
	manager.GetFactory().RegisterClass<Mesh>();

	Mesh mesh(true);

	mesh.m_primitiveType = 0;
	mesh.m_bShowNormals = false;
	mesh.m_bShowTangentSpaceMatrix = false;

	// BBox

	mesh.m_bAABBoxDirty = false;
	mesh.m_bbox.m_min = Point3f(0.0f, 0.0f, 0.0f);
	mesh.m_bbox.m_min = Point3f(1000.0f, 1000.0f, 1000.0f);

	Material m0("Mat0");
	m0.m_iBaseIndex = 0;
	m0.m_iStartIndex = 0;
	m0.m_iEndIndex = 5;
	m0.m_mesh = &mesh;

	Material m1("Mat1");
	m1.m_iBaseIndex = 0;
	m1.m_iStartIndex = 5;
	m1.m_iEndIndex = 10;
	m1.m_mesh = &mesh;

	// Materiales

	mesh.m_mats.Resize(2, Material(""));
	mesh.m_mats[0] = m0;
	mesh.m_mats[1] = m1;

	// Vertices

	mesh.m_vertices.Resize(1000, Point3f(0.0f, 0.0f, 0.0f));
	for(int i = 0; i < 1000; i++)
	{
		mesh.m_vertices[i] = Point3f(500.0f, 600.0f, 700.0f);
	}

	// Normals

	mesh.m_normals.Resize(1000, Point3f(0.0f, 0.0f, 0.0f));
	for(int i = 0; i < 1000; i++)
	{
		mesh.m_normals[i] = Point3f(1.0f, 0.0f, 0.0f);
	}

	//// Color Channel

	ColorChannel cc(1000);
	for(int i = 0; i < 1000; i++)
	{
		cc.m_colors[i] = 0xff;
	}

	mesh.m_colors.Resize(1, cc);

	//// Texture channels

	mesh.m_texCoords.Resize(2, TexChannel(0));

	TexChannel ch0(1000);
	for(int i = 0; i < 1000; i++)
	{
		ch0.m_texCoords[i] = Point3f(0.0f, 0.0f, 0.0f);
	}

	TexChannel ch1(1000);
	for(int i = 0; i < 1000; i++)
	{
		ch1.m_texCoords[i] = Point3f(1.0f, 1.0f, 0.0f);
	}

	mesh.m_texCoords[0] = ch0;
	mesh.m_texCoords[1] = ch1;

	//// Tangents

	mesh.m_tangents.Resize(1000, Point3f(0.0f, 0.0f, 0.0f));
	for(int i = 0; i < 1000; i++)
	{
		mesh.m_tangents[i] = Point3f(1.0f, 0.0f, 0.0f);
	}

	//// Binormals

	mesh.m_binormals.Resize(1000, Point3f(0.0f, 0.0f, 0.0f));
	for(int i = 0; i < 1000; i++)
	{
		mesh.m_binormals[i] = Point3f(1.0f, 0.0f, 0.0f);
	}

	//// Indices

	mesh.m_indices.Resize(2000, 0);
	for(int i = 0; i < 2000; i++)
	{
		mesh.m_indices[i] = static_cast<short>(i);
	}

	manager.Save("sample8.class", mesh);

	Resource<Mesh > mesh_d;
	manager.Load("sample8.class", mesh_d);
	mesh_d.Release();

}