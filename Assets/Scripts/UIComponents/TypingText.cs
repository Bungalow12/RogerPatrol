using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TypingText : MonoBehaviour
 {
	 [SerializeField]
	 private Text textComponent;

	 [SerializeField]
	 private AudioSource typingSound;

	 [SerializeField]
	 private float typingSpeed = 0.05f; // 20 characters per second.
	 
	 [SerializeField]
	 private bool hideWhenDone = true;

	 [SerializeField]
	 private float hideDelay = 2.0f;

	 private Action onShown;

	 private Coroutine coroutine;


	 private IEnumerator DoTypeText(string text)
	 {
		 for(int i = 0; i < text.Length; ++i)
		 {
			 this.textComponent.text += text[i];
			 if (!Char.IsWhiteSpace(text[i]))
			 {
			 	this.typingSound.Play();
			 }
			 yield return new WaitForSeconds(typingSpeed);
		 }

		 if(this.hideWhenDone)
		 {
		 	Invoke("Hide", this.hideDelay);
		 }
	 }

	 private void Hide()
	 {
		 this.gameObject.SetActive(false);
	 }

	 public void TypeText(string text)
	 {
		 this.textComponent.text = "";

		 if(this.coroutine != null)
		 {
		 	StopCoroutine(coroutine);
		 }

		 this.coroutine = StartCoroutine(DoTypeText(text.Replace("{n}", Environment.NewLine)));
	 }

	public void OnShown()
	{
		if(this.onShown != null)
		{
			this.onShown();
		}
	}

	public void Show(Action onShown)
	{		 
		this.onShown = onShown;
		this.gameObject.SetActive(true);
	}

	public void Stop()
	{
		if(this.coroutine != null)
		{
			StopCoroutine(this.coroutine);
		}
	}
}
