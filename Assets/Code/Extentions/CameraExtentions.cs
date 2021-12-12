using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public static class CameraExtentions
    {
        public static bool Visible(this Camera camera, Collider collider)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
        }
    } 
}
