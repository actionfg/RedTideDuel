using System.Collections.Generic;
using UnityEngine;


    public class TrackableTaggedObjectInfo
    {
        public string tagName;
        public Texture objectBlipTexture;
        public float objectBlipSize;
        public bool trackInRuntime = false;
        [HideInInspector] public List<TrackableObject> objectList;

        public List<TrackableObject> GetObjectList()
        {
            return objectList;
        }

        public void SetObjectList(List<TrackableObject> list)
        {
            objectList = list;
        }
    }
