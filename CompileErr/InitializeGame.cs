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
            List<ICardPack> CardPackDictList = CardPacksFactory.CardPackList;
            ICardPile IniCardPile = new CardPile();
            foreach (ICardPack CardPack in CardPackDictList)
            {
                if (CardPack.NameID == IniCardPileName)
                {
                    foreach (ICard IniCard in CardPack.DefaultCards)
                    {
                        IniCardPile.CardList.Add(IniCard.CopyCard());
                    }
                    break;
                }
            }
            PresentPlayer = new HumanPlayer(null, IniCardPile);
        }
        //��������Ϸ����ʼ�����
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
        }
        static internal void LoadEffect()
        {
            AEEFactory.EffectDict["Poison"] = new Effects.Poison();
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
                            foreach (XmlNode actForChoice in aChoice.SelectNodes("./Action")!)
                            {
                                newEvent.Actions[thisName].Add(actForChoice["NameID"]!.InnerText);
                                actValDict[actForChoice["NameID"]!.InnerText] = int.Parse(actForChoice["Value"]!.InnerText);
                            }
                            newEvent.keyValuePairs[thisName] = actValDict;
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
        public static List<List<IStage>> Lay1Group = new List<List<IStage>>();
        public static List<List<IStage>> Lay2Group = new List<List<IStage>>();
        //public static List<List<IStage>> Lay3Group = new List<List<IStage>>();
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
