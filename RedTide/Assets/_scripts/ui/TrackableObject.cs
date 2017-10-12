using UnityEngine;


    public class TrackableObject
    {
        public GameObject obj;
        public Transform blip;

        public TrackableObject(GameObject obj, Transform blip)
        {
            this.obj = obj;
            this.blip = blip;
        }

    }
