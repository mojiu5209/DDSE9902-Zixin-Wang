using UnityEngine;

public class FurnitureResetManager : MonoBehaviour
{
    public FurnitureReset[] furnitureList;   // 把所有家具拖进来

    public void ResetAllFurniture()
    {
        foreach (var f in furnitureList)
        {
            if (f != null)
                f.ResetFurniture();
        }
    }
}
