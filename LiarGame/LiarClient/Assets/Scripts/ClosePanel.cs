using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
   public Button btnClose;
   public GameObject panel;
   public GameObject chatPanel;
   public GameObject votePanel;


   private void Awake()
   {
      btnClose.onClick.AddListener(() => {
         chatPanel.SetActive(true);
         panel.SetActive(false);
         votePanel.SetActive(false);
      });
   }
}