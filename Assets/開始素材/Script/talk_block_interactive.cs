using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class talk_block_interactive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("show");
		gameObject.transform.GetChild (1).gameObject.GetComponent<Image> ().color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("hide");
		gameObject.transform.GetChild (1).gameObject.GetComponent<Image> ().color = Color.white;
    }

    public void start_button()
    {
        SceneManager.LoadScene(1);
    }

    public void set_button()
    {
        Debug.Log("Test");
    }

    public void exit_button()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
