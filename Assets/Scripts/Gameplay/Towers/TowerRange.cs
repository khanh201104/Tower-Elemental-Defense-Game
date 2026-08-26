using UnityEngine;

[RequireComponent(typeof(TowerAttack), typeof(TowerController))]
public class TowerRange : MonoBehaviour
{
    [Header("Tham Chiếu Vòng Tròn Tầm Bắn")]
    public GameObject rangeCircle; 

    private TowerAttack towerAttack;
    private TowerController towerController;

    void Start()
    {
        towerAttack = GetComponent<TowerAttack>();
        towerController = GetComponent<TowerController>();

        if (rangeCircle != null && towerAttack != null)
        {
            float currentRange = towerAttack.range; 
            rangeCircle.transform.localScale = new Vector3(currentRange * 2f, currentRange * 2f, 1f);
            
            // Chỉ hiển thị nếu nút tổng đang bật VÀ tháp đã được đặt xuống sân
            rangeCircle.SetActive(GameplayCanvasController.IsGlobalRangeVisible && towerController.isOperational);
        }
    }

    public void ShowRange(bool isGlobalShow)
    {
        if (rangeCircle != null)
        {
            // Nếu người chơi bấm bật, nhưng tháp đang ở hàng chờ (Inactive) -> Từ chối hiện
            if (isGlobalShow && towerController != null && !towerController.isOperational)
            {
                rangeCircle.SetActive(false);
                return;
            }

            if (isGlobalShow && towerAttack != null)
            {
                float currentRange = towerAttack.range;
                rangeCircle.transform.localScale = new Vector3(currentRange * 2f, currentRange * 2f, 1f);
            }

            rangeCircle.SetActive(isGlobalShow);
        }
    }
}