using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OpenHellion.IO;
using OpenHellion.Networking;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ZeroGravity.UI
{
	public class GlosseryMenu : MonoBehaviour
	{
		public Glossery Glossery;

		private GlosseryUI glosseryUI;

		private GlossaryGroupOption selectedGroup;

		[SerializeField]
		private Sprite missingSpritePlaceholder;

		[SerializeField]
		private Color normalButtonColor;

		[SerializeField]
		private Color pressedButtonColor;

		[SerializeField]
		private GameObject GroupButtonPref;

		public ScrollRect GroupScroll;

		public Transform GroupHolder;

		public GlossaryGroupUI GroupUI;

		public GlossaryItemUI ItemUI;

		[SerializeField]
		private GameObject TooltipPanel;

		[SerializeField]
		private Text TooltipText;

		[SerializeField]
		private GameObject LongDescriptionPanel;

		[SerializeField]
		private Image LongDescriptionImage;

		[SerializeField]
		private Text LongDescriptionName;

		[SerializeField]
		private Text LongDescriptionDescription;

		private void Start()
		{
			Glossery = JsonSerialiser.LoadResource<Glossery>("Data/Glossery");
			glosseryUI = new GlosseryUI();
			glosseryUI.Glossary = Glossery;
			glosseryUI.AllGroups = new List<GlossaryGroupOption>();
			foreach (GlosseryGroup glossaryGroup in Glossery.GlossaryGroups)
			{
				GameObject gameObject = Object.Instantiate(GroupButtonPref, GroupButtonPref.transform.parent);
				gameObject.transform.Find("Text").GetComponent<Text>().text = glossaryGroup.GroupName;
				gameObject.SetActive(value: true);
				GlossaryGroupOption newGrpUI = new GlossaryGroupOption();
				newGrpUI.ButtonImage = gameObject.transform.Find("Active").GetComponent<Image>();
				newGrpUI.GlossaryGrp = glossaryGroup;
				newGrpUI.AllSubGroups = new List<GlossaryGroupUI>();
				glosseryUI.AllGroups.Add(newGrpUI);
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					ChangeGroup(newGrpUI);
				});
				foreach (GlosserySubGroup subGroup in glossaryGroup.SubGroups)
				{
					GlossaryGroupUI glossaryGroupUI = Object.Instantiate(GroupUI, GroupHolder);
					glossaryGroupUI.GlossarySubGrp = subGroup;
					glossaryGroupUI.Name.text = subGroup.SubGroupName.ToUpper();
					glossaryGroupUI.AllItems = new List<GlossaryItemUI>();
					glossaryGroupUI.gameObject.SetActive(value: false);
					newGrpUI.AllSubGroups.Add(glossaryGroupUI);
					foreach (GlosseryItem item in subGroup.Items)
					{
						GlossaryItemUI glossaryItemUI = Object.Instantiate(ItemUI, glossaryGroupUI.ItemsHolder);
						glossaryItemUI.gameObject.SetActive(value: true);
						glossaryItemUI.Menu = this;
						glossaryItemUI.GlossaryItm = item;
						glossaryGroupUI.AllItems.Add(glossaryItemUI);
						glossaryItemUI.ItemName.text = item.Name;
						Texture2D texture2D = Resources.Load("UI/GlosseryImages/" + item.ImagePath) as Texture2D;
						if (texture2D != null)
						{
							Sprite sprite = (glossaryItemUI.Sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 1f, 0u, SpriteMeshType.Tight));
						}
						if (glossaryItemUI.Sprite != null)
						{
							glossaryItemUI.Image.sprite = glossaryItemUI.Sprite;
						}
						else
						{
							glossaryItemUI.Image.sprite = missingSpritePlaceholder;
						}
					}
				}
			}
			selectedGroup = glosseryUI.AllGroups[0];
			ChangeGroup(selectedGroup);
		}

		private void Update()
		{
			ShowTooltip();
		}

		public void ChangeGroup(GlossaryGroupOption groupUi)
		{
			if (selectedGroup != null)
			{
				foreach (GlossaryGroupUI allSubGroup in selectedGroup.AllSubGroups)
				{
					allSubGroup.gameObject.SetActive(value: false);
				}
				selectedGroup.ButtonImage.color = normalButtonColor;
			}
			selectedGroup = groupUi;
			foreach (GlossaryGroupUI allSubGroup2 in groupUi.AllSubGroups)
			{
				allSubGroup2.gameObject.SetActive(value: true);
			}
			selectedGroup.ButtonImage.color = pressedButtonColor;
			GroupScroll.normalizedPosition = new Vector2(0f, 1f);
			Canvas.ForceUpdateCanvases();
		}

		public void ShowTooltip()
		{
			GameObject gameObject = HandleUtility.PickGameObject(Mouse.current.position.ReadValue(), out _);
			if (gameObject != null)
			{
				GlossaryItemUI componentInParent = gameObject.GetComponentInParent<GlossaryItemUI>();
				if (componentInParent != null)
				{
					RectTransform component = TooltipPanel.GetComponent<RectTransform>();
					Vector2 localPoint = Vector2.zero;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), Client.Instance.CanvasManager.Canvas.worldCamera, out localPoint);
					component.transform.localPosition = localPoint;
					TooltipText.text = componentInParent.GlossaryItm.DescriptionShort;
					TooltipPanel.SetActive(value: true);
				}
				else
				{
					TooltipPanel.SetActive(value: false);
				}
			}
			else
			{
				TooltipPanel.SetActive(value: false);
			}
		}

		public void ShowLongDescription(GlossaryItemUI item)
		{
			LongDescriptionPanel.SetActive(value: true);
			LongDescriptionName.text = item.GlossaryItm.Name;
			if (item.Sprite != null)
			{
				LongDescriptionImage.sprite = item.Sprite;
			}
			else
			{
				LongDescriptionImage.sprite = missingSpritePlaceholder;
			}
			LongDescriptionDescription.text = item.GlossaryItm.DescriptionLong;
		}

		public void HideLongDescription()
		{
			LongDescriptionPanel.SetActive(value: false);
		}
	}
}
