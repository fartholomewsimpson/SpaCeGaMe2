using UnityEngine;

namespace PlanetGeneration
{
    public static class MeshGenerationUtils
    {
        public static Vector3[] GeneratePlaneVertices(Vector3 position, int width, int definition)
        {
            int verticesWidth = GetVerticesWidth(width, definition);
            var vertices = new Vector3[verticesWidth * verticesWidth];

            var topLeft = new Vector3(
                position.x - (width/2f),
                position.y - (width/2f),
                position.z);
            float fDef = (float)definition;

            for (int i = 0; i < verticesWidth; i++)
            {
                for (int j = 0; j < verticesWidth; j++)
                {
                    var vertexIndex = (verticesWidth * i) + j;
                    vertices[vertexIndex] = new Vector3(
                        topLeft.x + (i / fDef),
                        topLeft.y + (j / fDef),
                        position.z);
                }
            }
            return vertices;
        }

        public static int[] GeneratePlaneTriangles(int width, int definition)
        {
            int verticesWidth  = GetVerticesWidth(width, definition);
            var trianglesSqrWidth = (verticesWidth-1) * (verticesWidth-1);
            var triangles = new int[trianglesSqrWidth * 6];
            int verticesIndex = 0;

            for (int i = 0; i < triangles.Length; i+=6)
            {
                if ((verticesIndex+1) % verticesWidth == 0)
                    verticesIndex++;

                triangles[i] = verticesIndex;
                triangles[i+1] = verticesIndex+1;
                triangles[i+2] = verticesIndex+1+verticesWidth;

                triangles[i+3] = verticesIndex;
                triangles[i+4] = verticesIndex+1+verticesWidth;
                triangles[i+5] = verticesIndex+verticesWidth;

                verticesIndex++;
            }
            return triangles;
        }

        public static int GetVerticesWidth(int width, int definition)
        {
            // subtract (definition-1) to exclude adding additional definition points after the last row/column
            return ((width+1) * definition) - (definition-1);
        }
    }
}
