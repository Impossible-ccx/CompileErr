using Effects;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
namespace BackYard
{
    static public class GameManager
    {
        //ͨѶ��
        //ȫ��Ϸ���̵Ĺ�����
        //���к����ķ�����Ҫд��ʱ��ʱ������·��������Ƕ�ʷɽ��Ϊ����������ˡ���༸ʮ����Ա��һ�̰�
        //����һ�����iniNewGame��ʼ�����ʼ��
        //����ṩһ�����ڽ�����Ϸ�ķ����������Ǹ���ʱ��ע��Ϸ�������ֶ�
        static bool IsGameEnd = false;
        static public int FloorDepth { get; private set; }
        //������ֵ����ͬ������в�ͬ����
        static public int init = 0;
        static public IBattleManager? battleManager {  get; private set; }
        static public IHumanPlayer? PresentPlayer {  get; private set; }
        //�����Ϣ��������
        static public List<ICardPack> DisableCardPacks { get; set; } = new List<ICardPack>();
        //δ���õĿ����������õĿ���û�ã�����������������Ч��
        //loadxmlʱ���Ѿ����룬�����ٴγ�ʼ��
        static public List<ICard> EnableCards { get; set; } = new List<ICard>();
        //���õĿ������ɽ���ʱ����������
        static public void EnterStage(IStage target)
        {
            try
            {
                if (target!.type == 1)
                {
                    IBattle Target = (target as IBattle)!;
                    battleManager = new BattleManager();
                    battleManager!.StartBattle(Target);
                }
                else if(target!.type == 2)
                {
                    Event theEvent = (target as Event)!;
                    if (target!.Tag == "Event")
                    {
                        ;
                    }
                    else if(target!.Tag == "DestoryCard")
                    {
                        foreach(ICard aCard in PresentPlayer!.PresentCardPile.CardList)
                        {
                            string ActionNumStr = (theEvent.Choices.Count + 1).ToString();
                            theEvent.Choices.Add(ActionNumStr);
                            theEvent.Description += Environment.NewLine;
                            theEvent.Description += ActionNumStr;
                            theEvent.Description += aCard.Name;
                            theEvent.Actions[ActionNumStr] = ["DestoryCard"];
                            theEvent.keyValuePairs[ActionNumStr] = new Dictionary<string, double>();
                            theEvent.keyValuePairs[ActionNumStr]["DestoryCard"] = 0;
                            theEvent.keyArgsPairs[ActionNumStr] = new Dictionary<string, string?>();
                            theEvent.keyArgsPairs[ActionNumStr]["DestoryCard"] = aCard.ID;
                        }
                    }
                    else if(target!.Tag == "Shop")
                    {
                        List<ICard> shopList = new List<ICard>();
                        Random rand = new Random();
                        for(int i = 0; i< rand.Next(5, 8);i++)
                        {
                            int targetIndex = rand.Next(1000) % EnableCards.Count;
                            shopList.Add(EnableCards[targetIndex]);
                        }
                        foreach (ICard aCard in shopList)
                        {
                            string ActionNumStr = (theEvent.Choices.Count + 1).ToString();
                            theEvent.Choices.Add(ActionNumStr);
                            theEvent.Description += Environment.NewLine;
                            theEvent.Description += ActionNumStr;
                            theEvent.Description += aCard.Name;
                            theEvent.Actions[ActionNumStr] = ["BuyCard"];
                            theEvent.keyValuePairs[ActionNumStr] = new Dictionary<string, double>();
                            theEvent.keyValuePairs[ActionNumStr]["BuyCard"] = rand.Next(20, 50);
                            theEvent.Description += "  �۸�: " + theEvent.keyValuePairs[ActionNumStr]["BuyCard"].ToString();
                            theEvent.keyArgsPairs[ActionNumStr] = new Dictionary<string, string?>();
                            theEvent.keyArgsPairs[ActionNumStr]["BuyCard"] = aCard.ID;
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("E000 fail to start battle");
            }

        }
        //�����Ծ�
        static public void IniNewGame(string IniCardPileName)
        {
            //����Mod
            ModManager.LoadMods();
            //�������
            PresentPlayer = new HumanPlayer(null, new CardPile());
            PresentPlayer.MaxHP = 100;
            List<ICardPack> CardPackDictList = CardPacksFactory.CardPackList;
            ICardPile IniCardPile = new CardPile();
            //foreach (ICardPack CardPack in CardPackDictList)
            //{
            //    if (CardPack.NameID == IniCardPileName)
            //    {
            //        foreach (ICard IniCard in CardPack.DefaultCards)
            //        {
            //            IniCardPile.CardList.Add(IniCard.CopyCard());
            //        }
            //        break;
            //    }
            //}
            SelectNewCardPack(IniCardPileName);

        }
        //��������Ϸ����ʼ�����
        static public void SelectNewCardPack(string targetName)
        {
            ICardPack? target = null;
            foreach(ICardPack ttt in CardPacksFactory.CardPackList)
            {
                if(ttt.NameID == targetName)
                {
                    target = ttt; 
                    break;
                }
            }
            if(target == null)
            {
                throw new Exception("No such a card pack");
            }
            foreach(ICard card in target.DefaultCards)
            {
                PresentPlayer!.PresentCardPile.CardList.Add(card.CopyCard());
            }
            DisableCardPacks.Remove(target);
            foreach(ICard card in target.Cards)
            {
                EnableCards.Add(card.CopyCard());
            }
        }
    }
    static internal class ModManager
    {
        static internal readonly string WorkPath = System.IO.Directory.GetCurrentDirectory();
        static internal readonly string ModFolderPath = WorkPath + "/Mods/";
        static private string? ModPath;
        static private XmlDocument ModManagerXml = new XmlDocument();
        static internal void LoadMods()
        {
            //����card��enemy��floor
            ModManagerXml.Load(ModFolderPath + "ModManager.xml");
            XmlNode ModManagerXmlRoot = ModManagerXml.LastChild!;
            foreach(XmlNode aMod in ModManagerXmlRoot.ChildNodes)
            {
                ModPath = ModFolderPath + aMod.InnerText;
            }
            LoadCards();
            LoadAction();
            LoadEffect();
            LoadEnemys();
            LoadFloors();
        }
        static internal void LoadAction()
        {
            AEEFactory.ActionDict["Attack"] = new Actions.Attack();
            AEEFactory.ActionDict["AddHP"] = new Actions.AddHP();
            AEEFactory.ActionDict["FoldCard"] = new Actions.FoldCard();
            AEEFactory.ActionDict["AddPoison"] = new Actions.AddPoison();
            AEEFactory.ActionDict["AddDefense"] = new Actions.AddDefense();
            AEEFactory.ActionDict["DestoryCard"] = new Actions.DestoryCard();
            AEEFactory.ActionDict["BuyCard"] = new Actions.BuyCard();
        }
        static internal void LoadEffect()
        {
            AEEFactory.EffectDict["Poison"] = new Effects.Poison();
            AEEFactory.EffectDict["Defense"] = new Effects.Defense();
        }
        static private void LoadCards()
        {
            XmlDocument CardsXml = new XmlDocument();
            CardsXml.Load(ModPath + "Cards.xml");
            XmlNode root = CardsXml.LastChild!;
            foreach (XmlNode aCardPack in root.ChildNodes)
            {
                CardPack newCardPack = new CardPack();
                newCardPack.NameID = aCardPack["Name"]!.InnerText;
                XmlNodeList CardsList = aCardPack.SelectNodes("./Card")!;
                XmlNodeList DefaultCardsList = aCardPack.SelectNodes("./DefaultCard")!;
                foreach (XmlElement aCard in CardsList)
                {
                    Card newCard = new Card();
                    newCard.WhereThis = int.Parse(aCard["WhereThis"]!.InnerText);
                    newCard.Name = aCard["Name"]!.InnerText;
                    newCard.Description = aCard["Description"]!.InnerText;
                    newCard.ImagePath = ModPath + "Images/" + aCard["ImagePath"]!.InnerText;
                    newCard.ID = aCard["ID"]!.InnerText;
                    newCard.Cost = int.Parse(aCard["Cost"]!.InnerText);
                    foreach (XmlNode xmlNode in aCard["ID"]!.ChildNodes)
                    {
                        newCard.Tags.Add(xmlNode.InnerText);
                    }
                    newCard.Delay = int.Parse(aCard["Delay"]!.InnerText);
                    foreach (XmlNode aActionXml in aCard["Action"]!.ChildNodes)
                    {
                        newCard.activeAcions.Add(aActionXml["NameID"]!.InnerText);
                        newCard.actionValue[aActionXml["NameID"]!.InnerText] = double.Parse(aActionXml["Value"]!.InnerText);
                    }
                    CardPacksFactory.CardDict[newCard.ID] = newCard;
                    newCardPack.Cards.Add(newCard);
                }
                foreach (XmlElement aDefaultCardXml in DefaultCardsList)
                {
                    ICard aCard = CardPacksFactory.CardDict[aDefaultCardXml.InnerText].CopyCard();
                    if (aCard != null)
                    {
                        newCardPack.DefaultCards.Add(aCard);
                    }
                }
                CardPacksFactory.CardPackList.Add(newCardPack);
                GameManager.DisableCardPacks.Add(newCardPack);
            }
        }
        static private void LoadEnemys()
        {
            XmlDocument EnemysXml = new XmlDocument();
            EnemysXml.Load(ModPath + "Enemys.xml");
            XmlNode root = EnemysXml.LastChild!;
            foreach (XmlNode aEnemyXml in root.ChildNodes)
            {
                Enemy enemy = new Enemy();
                enemy.Name = aEnemyXml["Name"]!.InnerText;
                enemy.ID = aEnemyXml["ID"]!.InnerText;
                enemy.HP = int.Parse(aEnemyXml["HP"]!.InnerText);
                enemy.MaxHP = enemy.HP;
                foreach (XmlNode aEffectXml in aEnemyXml["Effects"]!)
                {
                    IEffect newEffect = AEEFactory.EffectDict[aEffectXml["IDName"]!.InnerText].Copy();
                    newEffect.Level = int.Parse(aEffectXml["Level"]!.InnerText);
                    enemy.EffectBox.Add(newEffect);
                }
                foreach(XmlNode aLogicXml in aEnemyXml["Logic"]!.ChildNodes)
                {
                    EnemyLogic aLogic = new EnemyLogic();
                    aLogic.value = int.Parse(aLogicXml["Value"]!.InnerText);
                    aLogic.delay = int.Parse(aLogicXml["Delay"]!.InnerText);
                    aLogic.IniDelay = aLogic.delay;
                    aLogic.IsSingle = aLogicXml["IsSingle"]!.InnerText == "true";
                    aLogic.action = AEEFactory.ActionDict[aLogicXml["Action"]!.InnerText];
                    enemy.Logic.Add(aLogic);
                }
                AEEFactory.EnemyDict[enemy.ID] = enemy;
            }
        }
        static private void LoadFloors()
        {
            XmlDocument FloorsXml = new XmlDocument();
            FloorsXml.Load(ModPath + "Floors.xml");
            XmlNode root = FloorsXml.LastChild!;
            foreach (XmlNode aFloorXml in root.ChildNodes)
            {
                List<List<IStage>>? targetGroup;
                List<IStage> newFloor = new List<IStage>();
                if (aFloorXml["LayLevel"]!.InnerText == "1")
                {
                    targetGroup = FloorFactoy.Lay1Group;
                }
                else if (aFloorXml["LayLevel"]!.InnerText == "2"){
                    targetGroup = FloorFactoy.Lay2Group;
                }
                else if (aFloorXml["LayLevel"]!.InnerText == "3")
                {
                    targetGroup = FloorFactoy.Lay3Group;
                }
                else
                {
                    targetGroup = null;
                    throw new Exception("???stageGroup");
                }
                XmlNodeList stages = aFloorXml.SelectNodes("./Stage")!;
                foreach(XmlNode stage in stages)
                {
                    IStage? newStage;
                    if (stage["Type"]!.InnerText == "1")
                    {
                        BattleStage newBattle = new BattleStage();
                        newStage = newBattle;
                        newBattle.type = 1;
                        newBattle.Tag = stage["Tag"]!.InnerText;
                        newBattle.Reward = int.Parse(stage["Reward"]!.InnerText);
                        foreach(XmlNode enemy in stage["EnemyList"]!.ChildNodes)
                        {
                            newBattle.EnemyList.Add((AEEFactory.EnemyDict[enemy.InnerText].Copy() as IEnemy)!);
                        }
                        foreach(XmlNode card in stage["RewardCard"]!.ChildNodes)
                        {
                            newBattle.RewardCard.Add((CardPacksFactory.CardDict[card.InnerText].CopyCard())!);
                        }
                    }
                    else if (stage["Type"]!.InnerText == "2")
                    {
                        Event newEvent = new Event();
                        newStage = newEvent;
                        newEvent.type = 2;
                        newEvent.Tag = stage["Tag"]!.InnerText;
                        newEvent.Description = stage["Description"]!.InnerText;
                        foreach(XmlNode aChoice in stage["Choices"]!.ChildNodes)
                        {
                            string thisName = aChoice["Name"]!.InnerText;
                            newEvent.Choices.Add(new string(thisName));
                            newEvent.Actions[thisName] = new List<string>();
                            Dictionary<string, double> actValDict = new Dictionary<string, double>();
                            Dictionary<string, string?> actArgsDict = new Dictionary<string, string?>();
                            foreach (XmlNode actForChoice in aChoice.SelectNodes("./Action")!)
                            {
                                newEvent.Actions[thisName].Add(actForChoice["NameID"]!.InnerText);
                                actValDict[actForChoice["NameID"]!.InnerText] = int.Parse(actForChoice["Value"]!.InnerText);
                                actArgsDict[actForChoice["NameID"]!.InnerText] = null;
                            }
                            newEvent.keyValuePairs[thisName] = actValDict;
                            newEvent.keyArgsPairs[thisName] = actArgsDict;
                        }
                    }
                    else
                    {
                        newStage = null;
                        throw new Exception("???stage");
                    }
                    newFloor.Add(newStage);
                }
                targetGroup.Add(newFloor);
            }
        }

    }
    static public class FloorFactoy
    {
        public static List<IStage> Map = new List<IStage>();
        public static List<List<IStage>> Lay1Group = new List<List<IStage>>();
        public static List<List<IStage>> Lay2Group = new List<List<IStage>>();
        public static List<List<IStage>> Lay3Group = new List<List<IStage>>();
    }
    static public class CardPacksFactory
    {
        public static List<ICardPack> CardPackList = new List<ICardPack>();
        public static Dictionary<string, ICard> CardDict = new Dictionary<string, ICard>();
    }
    static public class AEEFactory
    {
        static public Dictionary<string, IAction> ActionDict { get; internal set; } = new Dictionary<string, IAction>();
        static public Dictionary<string, IEffect> EffectDict { get; internal set; } = new Dictionary<string, IEffect>();
        static public Dictionary<string, IEnemy> EnemyDict { get; internal set;} = new Dictionary<string, IEnemy>();
    }

}
