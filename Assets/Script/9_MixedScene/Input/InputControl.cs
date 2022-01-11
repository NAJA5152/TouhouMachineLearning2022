using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class InputControl : MonoBehaviour
    {
        public float height;
        Ray ray;
        public float PassPressTime;
        void Update()
        {
            GetFocusTarget();
            MouseEvent();
            KeyBoardEvent();
        }
        private void GetFocusTarget()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] Infos = Physics.RaycastAll(ray);
            if (Infos.Length > 0)
            {
                for (int i = 0; i < Infos.Length; i++)
                {
                    if (Infos[i].transform.GetComponent<SingleRowManager>() != null)
                    {
                        AgainstInfo.PlayerFocusRegion = Infos[i].transform.GetComponent<SingleRowManager>();
                        AgainstInfo.FocusPoint = Infos[i].point;
                        break;
                    }
                    AgainstInfo.PlayerFocusRegion = null;
                }
            }
            float distance = (height - ray.origin.y) / ray.direction.y;
            AgainstInfo.dragToPoint = ray.GetPoint(distance);
            Debug.DrawLine(ray.origin, AgainstInfo.dragToPoint, Color.red);
            Debug.DrawRay(ray.origin, ray.direction, Color.white);
        }
        private void KeyBoardEvent()
        {

            if (Input.GetKey(KeyCode.Space) && Info.AgainstInfo.IsMyTurn)
            {
                PassPressTime += Time.deltaTime;
                if (PassPressTime > 2)
                {
                    Command.NetCommand.AsyncInfo(NetAcyncType.Pass);
                    Info.AgainstInfo.isPlayerPass = true;
                    //Command.GameUI.UiCommand.SetCurrentPass();
                    PassPressTime = 0;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && Info.AgainstInfo.IsMyTurn)
            {
                PassPressTime = 0;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _ = Command.GameUI.NoticeCommand.ShowAsync("确认投降？", okAction: Command.StateCommand.Surrender);
            }
        }
        private void MouseEvent()
        {
            if (Input.GetMouseButtonDown(0) && Info.AgainstInfo.IsMyTurn)
            {
                if (AgainstInfo.IsWaitForSelectRegion)
                {
                    AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRegion.RowRank;
                }
                //处理选择单位的箭头
                if (AgainstInfo.IsWaitForSelectUnits && AgainstInfo.playerFocusCard != null && !AgainstInfo.playerFocusCard.IsGray)
                {
                    Card playerFocusCard = AgainstInfo.playerFocusCard;
                    if (!AgainstInfo.SelectUnits.Contains(playerFocusCard))
                    {
                        //Debug.LogError("add" + playerFocusCard);
                        AgainstInfo.SelectUnits.Add(playerFocusCard);
                        Command.GameUI.UiCommand.CreatFixedArrow(playerFocusCard);
                    }
                    else
                    {
                        //Debug.LogError("remove" + playerFocusCard);
                        AgainstInfo.SelectUnits.Remove(playerFocusCard);
                        Command.GameUI.UiCommand.DestoryFixedArrow(playerFocusCard);
                    }
                }
                if (AgainstInfo.IsWaitForSelectLocation)
                {
                    if (AgainstInfo.PlayerFocusRegion != null && AgainstInfo.PlayerFocusRegion.CanBeSelected)
                    {
                        AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRegion.RowRank;
                        AgainstInfo.SelectRank = AgainstInfo.PlayerFocusRegion.Location;
                    }
                }
            }
        }
    }
}