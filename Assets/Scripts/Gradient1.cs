using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient1 : BaseMeshEffect
{
    //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
    [SerializeField]
    public Color32 topColor = Color.white;
    [SerializeField]
    public Color32 bottomColor = Color.black;
    //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!this.IsActive())
            return;
        List<UIVertex> vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);
        ModifyVertices(vertexList);
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
    public void ModifyVertices(List<UIVertex> vertexList)
    {
        int count = vertexList.Count;
        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        if (vertexList.Count == 0)
        {
            return;
        }
        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        float bottomY = vertexList[0].position.y;

        float topY = vertexList[0].position.y;
        for (int i = 1; i < count; i++)
        {
            float y = vertexList[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }
        float uiElementHeight = topY - bottomY;
        for (int i = 0; i < count; i++)
        {
            UIVertex uiVertex = vertexList[i];
            uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
            vertexList[i] = uiVertex;
        }
    }
}