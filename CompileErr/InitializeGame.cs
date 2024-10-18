using Effects;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
namespace BackYard
{
    static public class GameManager
    {
        //ͨѶ��
        //ȫ��Ϸ���̵Ĺ�����
        //���к����ķ�����Ҫд��ʱ��ʱ������·��������Ƕ�ʷɽ��Ϊ����������ˡ���༸ʮ����Ա��һ�̰�
        //����һ�����init��ֵ�ͻῪʼ��ʼ���ˣ���
        //����ṩһ�����ڽ�����Ϸ�ķ����������Ǹ���ʱ��ע��Ϸ�������ֶ�
        static bool IsGameEnd = false;
        static public int FloorDepth { get; private set; }
        //������ֵ����ͬ������в�ͬ����
        static public int init = 0;
        static GameManager()
        {
            ModManager.LoadMods();
        }
        static public IFloorManager? floorManager;
        static public IBattleManager? battleManager;
        static public IHumanPlayer humanPlayer;
        //�����Ϣ��������
        static public List<ICardPack> DisableCardPacks { get; set; }
        //δ���õĿ����������õĿ���û�ã�����������������Ч��
        //loadxmlʱ���Ѿ����룬�����ٴγ�ʼ��
        static public List<ICard> EnableCards { get; set; }
        //���õĿ������ɽ���ʱ����������

        static public Dictionary<string,IAction> ActionDict = new Dictionary<string,IAction>();
        static public Dictionary<string,IEffect> EffectDict = new Dictionary<string,IEffect>();
        static public void EnterNextFloor(ICardPack? newCardPack)
        {
            //�ո�newgameҲҪ����
            //���ᰴ��һ���ķ�����ȡ�ļ��еĵ�ͼ����
            //�������һ�㣬����endgame
            int t = new Random().Next();
            switch (FloorDepth)
            {
                case 0:
                    t = t % FloorFactoy.Lay1Group.Count;
                    FloorDepth++;
                    floorManager = FloorManager.CreateFloor(FloorFactoy.Lay1Group[t]);     
                    break;
                case 1:
                    t = t % FloorFactoy.Lay2Group.Count;
                    FloorDepth++;
                    floorManager = FloorManager.CreateFloor(FloorFactoy.Lay2Group[t]);
                    break;
                case 2:
                    IsGameEnd = true;
                    floorManager = null;
                    break;
            }
        }
        static public void EnterStage(IStage target)
        {
            try
            {
                if (target!.type == 1)
                {
                    IBattle Target = (target as IBattle)!;
                    floorManager!.IniBattle(ref Target);
                    battleManager!.StartBattle(Target);
                }
            }
            catch
            {
                throw new Exception("E000 fail to start battle");
            }

        }
        //�����Ծ�
    }
    static internal class ModManager
    {
        static internal readonly string WorkPath = System.IO.Directory.GetCurrentDirectory();
        static internal readonly string ModFolderPath = WorkPath + "Mods/";
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
        }
        static internal void LoadAction()
        {
            GameManager.ActionDict["Attack"] = new Actions.Attack();
        }
        static internal void LoadEffect()
        {
            GameManager.EffectDict["Bleeding"] = new Effects.Bleeding();
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
                foreach(XmlElement aCard in CardsList)
                {
                    Card newCard = new Card();
                    newCard.Name = aCard["Name"]!.InnerText;
                    newCard.Description = aCard["Description"]!.InnerText;
                    newCard.ID = aCard["ID"]!.InnerText;
                    newCard.Cost = int.Parse(aCard["Cost"]!.InnerText);
                    foreach(XmlNode xmlNode in aCard["ID"]!.ChildNodes)
                    {
                        newCard.Tags.Add(xmlNode.InnerText);
                    }
                    newCard.Delay = int.Parse(aCard["Delay"]!.InnerText);
                    foreach(XmlNode aActionXml in aCard["Action"]!.ChildNodes)
                    {
                        newCard.activeAcions.Add(aActionXml["NameID"]!.InnerText);
                        newCard.actionValue[aActionXml["NameID"]!.InnerText] = double.Parse(aActionXml["Value"]!.InnerText);
                    }
                    CardPacksFactory.CardDict[newCard.Name] = newCard;
                    newCardPack.Cards.Add(newCard);
                }
                foreach(XmlElement aDefaultCardXml in DefaultCardsList)
                {
                    ICard aCard = CardPacksFactory.CardDict[aDefaultCardXml.InnerText].CopyCard();
                    if(aCard != null)
                    {
                        newCardPack.DefaultCards.Add(aCard);
                    }
                }
                CardPacksFactory.CardPackList.Add(newCardPack);
                GameManager.DisableCardPacks.Add(newCardPack);
            }
        }
    }
    static internal class FloorFactoy
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


}
