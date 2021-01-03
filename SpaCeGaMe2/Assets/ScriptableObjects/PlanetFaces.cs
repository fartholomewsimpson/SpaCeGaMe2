using System;
using System.Collections.Generic;
using PlanetGeneration;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName="Planet Faces", fileName="PlanetFaces", order=51)]
    public class PlanetFaces : ScriptableObject
    {
        public Face[] Faces
        {
            get { return faces; }
        }

        Dictionary<string, Action<Face[]>> onRefresh = new Dictionary<string, Action<Face[]>>();
        Face[] faces;

        public void Refresh(Face[] faces)
        {
            this.faces = faces;
            foreach (var subs in onRefresh)
                subs.Value(faces);
        }

        public void Subscribe(string name, Action<Face[]> callback)
        {
            onRefresh.Add(name, callback);
        }

        public void UnSubscribe(string name)
        {
            onRefresh?.Remove(name);
        }
    }
}
