using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGUI_2
{
    public partial class Chess_Main_Form : Form
    {
        List<PictureBox> lst_pb = new List<PictureBox>();
        bool player;                                                        /////true=whitePlaying false=blackPlaying;      
        PictureBox selSquare;
        PictureBox enPassent;
        bool specialMoves = false;
        Stack<MoveHistory> moveHistories = new Stack<MoveHistory>();
        Stack<MoveHistory> moveHistoriesTemp = new Stack<MoveHistory>();
        List<PictureBox> lstWhitePoint = new List<PictureBox>();
        List<PictureBox> lstBlackPoint = new List<PictureBox>();
        int whiteCounter;
        int blackCounter;
        int moveCounter = 0;
        TimeSpan whiteTime;
        TimeSpan blackTime;
        int whiteWin;
        int blackWin;
        public Chess_Main_Form()
        {
            InitializeComponent();
            SetDefaultValues();
        }
        private void SetDefaultValues()
        {
            selSquare = null;
            moveHistories.Clear();
            moveHistoriesTemp.Clear();
            lst_pb.Clear();
            lst_pb.AddRange(new List<PictureBox>(){a1,b1,c1,d1,e1,f1,g1,h1,
                                                   a2,b2,c2,d2,e2,f2,g2,h2,
                                                   a3,b3,c3,d3,e3,f3,g3,h3,
                                                   a4,b4,c4,d4,e4,f4,g4,h4,
                                                   a5,b5,c5,d5,e5,f5,g5,h5,
                                                   a6,b6,c6,d6,e6,f6,g6,h6,
                                                   a7,b7,c7,d7,e7,f7,g7,h7,
                                                   a8,b8,c8,d8,e8,f8,g8,h8,});
            foreach (PictureBox p in lst_pb)
                p.BackgroundImage = null;
            lstWhitePoint.Clear();
            lstWhitePoint.AddRange(new List<PictureBox> {whitePoint_1,whitePoint_2,whitePoint_3,whitePoint_4,
                                                         whitePoint_5,whitePoint_6,whitePoint_7,whitePoint_8,
                                                         whitePoint_9,whitePoint_10,whitePoint_11,whitePoint_12,
                                                         whitePoint_13,whitePoint_14,whitePoint_15,whitePoint_16});
            foreach (PictureBox item in lstWhitePoint)
                item.Image = null;
            lstBlackPoint.Clear();
            lstBlackPoint.AddRange(new List<PictureBox> { blackPoint_1,blackPoint_2,blackPoint_3,blackPoint_4,
                                                          blackPoint_5,blackPoint_6,blackPoint_7,blackPoint_8,
                                                          blackPoint_9,blackPoint_10,blackPoint_11,blackPoint_12,
                                                          blackPoint_13,blackPoint_14,blackPoint_15,blackPoint_16});
            foreach (PictureBox item in lstBlackPoint)
                item.Image = null;
            foreach(PictureBox item in lst_pb)
            {
                item.Image = null;
                item.Tag = "12";
            }
            for (int i = 8; i < 16; i++)        //white Pawn
            {
                lst_pb[i].Tag = 10;
                lst_pb[i].Image = Properties.Resources.Pieces_White_Pawn;
            }
            for (int i = 48; i < 56; i++)       //black Pawn
            {
                lst_pb[i].Tag = 11;
                lst_pb[i].Image = Properties.Resources.Pieces_Black_Pawn;
            }      
            lst_pb[0].Tag = 8; lst_pb[7].Tag = 8; lst_pb[56].Tag = 9; lst_pb[63].Tag = 9;
            lst_pb[1].Tag = 4; lst_pb[6].Tag = 4; lst_pb[57].Tag = 5; lst_pb[62].Tag = 5;
            lst_pb[2].Tag = 6; lst_pb[5].Tag = 6; lst_pb[58].Tag = 7; lst_pb[61].Tag = 7;
            lst_pb[3].Tag = 2; lst_pb[59].Tag = 3;
            lst_pb[4].Tag = 0; lst_pb[60].Tag = 1;

            lst_pb[0].Image=Properties.Resources.Pieces_White_Rook; lst_pb[7].Image = Properties.Resources.Pieces_White_Rook; lst_pb[56].Image = Properties.Resources.Pieces_Black_Rook; lst_pb[63].Image = Properties.Resources.Pieces_Black_Rook;
            lst_pb[1].Image=Properties.Resources.Pieces_White_Knight; lst_pb[6].Image = Properties.Resources.Pieces_White_Knight; lst_pb[57].Image = Properties.Resources.Pieces_Black_Knight; lst_pb[62].Image = Properties.Resources.Pieces_Black_Knight;
            lst_pb[2].Image=Properties.Resources.Pieces_White_Bishop; lst_pb[5].Image = Properties.Resources.Pieces_White_Bishop; lst_pb[58].Image = Properties.Resources.Pieces_Black_Bishop; lst_pb[61].Image = Properties.Resources.Pieces_Black_Bishop;
            lst_pb[3].Image=Properties.Resources.Pieces_White_Queen; lst_pb[59].Image=Properties.Resources.Pieces_Black_Queen;
            lst_pb[4].Image = Properties.Resources.Pieces_White_King; lst_pb[60].Image = Properties.Resources.Pieces_Black_King;
            btnUndo.Enabled = false;
            btnRedo.Enabled = false;
            wTimerDisplay.Text = "20:00";
            bkTimerDisplay.Text = "20:00";
            whiteCounter = 0;
            blackCounter = 0;
            LstBx_MoveHistory.Items.Clear();
            player = true;
            whiteTime = new TimeSpan(0, 20, 0);
            blackTime = new TimeSpan(0, 20, 0);
            TickSwitcher();
            label1.Text = "White's Turn";
            WhitePointCounter.Text = "0";
            BlackPointCounter.Text = "0";
            label2.Visible = false;
        } 
        private char GetPiece(string a)
        {
            switch (a)
            {
                case "0":
                case "1":
                    return 'K';
                case "2":
                case "3":
                    return 'Q';
                case "4":
                case "5":
                    return 'N';
                case "6":
                case "7":
                    return 'B';
                case "8":
                case "9":
                    return 'R';
                case "10":
                case "11":
                    return 'P';
                default:
                    return ' ';
            }
        }
        private void Click_event(PictureBox o)
        {
            label2.Visible = false;
            if (o.BackgroundImage != null && o.Tag.ToString() != "0" && o.Tag.ToString() != "1")  //Clicked on an orange or blue square
            {
                if (o.Name.ToString()!=selSquare.Name.ToString())    //This means the square is not blue
                {
                    if (moveHistoriesTemp.Any())
                    {
                        moveHistoriesTemp.Clear();
                        btnRedo.Enabled = false;
                    }
                    MoveHistory temp = new MoveHistory(0,0,null,null);
                    shamePiece shameTemp=new shamePiece();
                    if (o.Image != null)
                    {
                        shameTemp = new shamePiece(GetCoords(lst_pb.FindIndex(a => a == o)), o.Tag.ToString());
                        HallOfTheShame(true, player, o.Tag.ToString());
                    }
                    else if (specialMoves && GetCoords(lst_pb.FindIndex(a => a == o))[0] == GetCoords(lst_pb.FindIndex(a => a == enPassent))[0])
                    {
                        shameTemp = new shamePiece(GetCoords(lst_pb.FindIndex(a => a == enPassent)), enPassent.Tag.ToString());
                        HallOfTheShame(true, player, enPassent.Tag.ToString());
                        enPassent.Image = null;
                        enPassent.Tag = "12";
                        temp = new MoveHistory(lst_pb.FindIndex(a => a == selSquare), lst_pb.FindIndex(a => a == o), selSquare.Tag.ToString(), o.Tag.ToString());
                        temp.shamePiece = shameTemp;
                        temp.HistoryStringWriter(GetPiece(selSquare.Tag.ToString()), selSquare.Name.ToString(), GetPiece(o.Tag.ToString()), o.Name.ToString());
                        LstBx_MoveHistory.Items.Add(temp.HistoryString);
                        temp.EnPassentFlag = true;
                    }
                    if (!specialMoves)
                    {
                        temp = new MoveHistory(lst_pb.FindIndex(a => a == selSquare), lst_pb.FindIndex(a => a == o), selSquare.Tag.ToString(), o.Tag.ToString());
                        temp.shamePiece = shameTemp;
                        temp.HistoryStringWriter(GetPiece(selSquare.Tag.ToString()), selSquare.Name.ToString(), GetPiece(o.Tag.ToString()), o.Name.ToString());
                        LstBx_MoveHistory.Items.Add(temp.HistoryString);
                    }
                    moveHistories.Push(temp);
                    
                    if(o.Tag.ToString() != "12" || selSquare.Tag.ToString() =="10" || selSquare.Tag.ToString() == "11")
                    {
                        moveCounter = 0;
                    }
                    else
                    {
                        moveCounter++;
                        if (moveCounter > 49)
                        {
                            wtTimer.Stop();
                            bkTimer.Stop();
                            if (CustomMsg.Show("Tie!") == DialogResult.OK)
                                SetDefaultValues();
                        }
                    }
                    o.Image = selSquare.Image;
                    o.Tag = selSquare.Tag.ToString();
                    selSquare.Image = null;
                    selSquare.Tag = "12";
                    btnUndo.Enabled = true;
                    specialMoves = false;
                    ////////////castle//////////
                    if (LstBx_MoveHistory.Items[LstBx_MoveHistory.Items.Count - 1].ToString() == "Ke1-c1")
                    {
                        lst_pb[0].Image = null;
                        lst_pb[0].Tag = "12";
                        lst_pb[3].Image = Properties.Resources.Pieces_White_Rook;
                        lst_pb[3].Tag = "8";
                        temp.CastlingFlag = true;
                        temp.CastlingWhere = new bool[] { true, true };
                    }
                    else if (LstBx_MoveHistory.Items[LstBx_MoveHistory.Items.Count - 1].ToString() == "Ke1-g1")
                    {
                        lst_pb[7].Image = null;
                        lst_pb[7].Tag = "12";
                        lst_pb[5].Image = Properties.Resources.Pieces_White_Rook;
                        lst_pb[5].Tag = "8";
                        temp.CastlingFlag = true;
                        temp.CastlingWhere = new bool[] { true, false };
                    }
                    if (LstBx_MoveHistory.Items[LstBx_MoveHistory.Items.Count - 1].ToString() == "Ke8-c8")
                    {
                        lst_pb[56].Image = null;
                        lst_pb[56].Tag = "12";
                        lst_pb[59].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[59].Tag = "9";
                        temp.CastlingFlag = true;
                        temp.CastlingWhere = new bool[] { false, true };
                    }
                    else if (LstBx_MoveHistory.Items[LstBx_MoveHistory.Items.Count - 1].ToString() == "Ke8-g8")
                    {
                        lst_pb[63].Image = null;
                        lst_pb[63].Tag = "12";
                        lst_pb[61].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[61].Tag = "9";
                        temp.CastlingFlag = true;
                        temp.CastlingWhere = new bool[] { false, false };
                    }
                    ////////////En Passent//////////
                    if (o.Tag.ToString() == "11" && o.Name.ToString()[1] == '5' && selSquare.Name.ToString()[1] == '7')
                    {
                        if (lst_pb[lst_pb.FindIndex(a => a == o) - 1].Tag.ToString() == "10" || lst_pb[lst_pb.FindIndex(a => a == o) + 1].Tag.ToString() == "10")
                        {
                            enPassent = o;
                            specialMoves = true;
                        }
                    }        
                    else if (o.Tag.ToString() == "10" && o.Name.ToString()[1] == '4' && selSquare.Name.ToString()[1] == '2')
                    {
                        if (lst_pb[lst_pb.FindIndex(a => a == o) - 1].Tag.ToString() == "11" || lst_pb[lst_pb.FindIndex(a => a == o) + 1].Tag.ToString() == "11")
                        {
                            enPassent = o;
                            specialMoves = true;
                        }
                    }
                    //////////Pawn Promotion//////////
                    if (o.Name.ToString()[1] == '8' && player && o.Tag.ToString() == "10" || o.Name.ToString()[1] == '1' && !player && o.Tag.ToString() == "11")//pawn promotion
                    {
                        Form2 promote = new Form2(player);
                        DialogResult to = promote.ShowDialog();
                        if (to == DialogResult.OK)
                        {
                            if (player)
                            {
                                o.Tag = "2";
                                o.Image = Properties.Resources.Pieces_White_Queen;
                                temp.PawnPromotionTag = "2";
                                temp.PawnPromotionImage= Properties.Resources.Pieces_White_Queen;
                            }
                            else
                            {
                                o.Tag = "3";
                                o.Image = Properties.Resources.Pieces_Black_Queen;
                                temp.PawnPromotionTag = "3";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_Black_Queen;
                            }
                        }
                        if (to == DialogResult.Yes)
                        {
                            if (player)
                            {
                                o.Tag = "8";
                                o.Image = Properties.Resources.Pieces_White_Rook;
                                temp.PawnPromotionTag = "8";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_White_Rook;
                            }
                            else
                            {
                                o.Tag = "9";
                                o.Image = Properties.Resources.Pieces_Black_Rook;
                                temp.PawnPromotionTag = "9";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_Black_Rook;
                            }
                        }
                        if (to == DialogResult.No)
                        {
                            if (player)
                            {
                                o.Tag = "6";
                                o.Image = Properties.Resources.Pieces_White_Bishop;
                                temp.PawnPromotionTag = "6";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_White_Bishop;
                            }
                            else
                            {
                                o.Tag = "7";
                                o.Image = Properties.Resources.Pieces_Black_Bishop;
                                temp.PawnPromotionTag = "7";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_Black_Bishop;
                            }
                        }
                        if (to == DialogResult.Cancel)
                        {
                            if (player)
                            {
                                o.Tag = "4";
                                o.Image = Properties.Resources.Pieces_White_Knight;
                                temp.PawnPromotionTag = "4";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_White_Knight;
                            }
                            else
                            {
                                o.Tag = "5";
                                o.Image = Properties.Resources.Pieces_Black_Knight;
                                temp.PawnPromotionTag = "5";
                                temp.PawnPromotionImage = Properties.Resources.Pieces_Black_Knight;
                            }
                        }
                        temp.PawnPromotionFlag = true;
                    } 
                    player = !player;
                    if (IsCheck())
                    {
                        foreach (PictureBox p in lst_pb)
                            p.BackgroundImage = null;
                        FindOtherKing().BackgroundImage = Properties.Resources.red_Back;
                        if (!IsMate(o))
                        {
                            btnUndo.Enabled = false;
                            btnRedo.Enabled = false;
                            if (!player)
                            {
                                wtTimer.Stop();
                                bkTimer.Stop();
                                if (CustomMsg.Show("White Player Win!!!!!!!!!!!!") == DialogResult.OK)
                                {
                                    SetDefaultValues();
                                    whiteWin++;
                                    WhiteWinCounter.Text = whiteWin.ToString();
                                }  
                            }
                            else
                            {
                                wtTimer.Stop();
                                bkTimer.Stop();
                                if (CustomMsg.Show("Black Player Win!!!!!!!!!!!!") == DialogResult.OK)
                                {
                                    SetDefaultValues();
                                    blackWin++;
                                    BlackWinCounter.Text = blackWin.ToString();
                                }
                            }
                        }
                    }
                    else if (!IsMate(o))
                    {
                        foreach (PictureBox p in lst_pb)
                            p.BackgroundImage = null;
                        wtTimer.Stop();
                        bkTimer.Stop();
                        if (CustomMsg.Show("Stalemate (Tie)") == DialogResult.OK)
                            SetDefaultValues();
                    }
                    else
                    {
                        foreach (PictureBox p in lst_pb)
                            p.BackgroundImage = null;
                    }
                    if (ThreefoldRepetition())
                        if(CustomMsg.Show("Draw!!!") == DialogResult.OK)
                            SetDefaultValues();
                    TickSwitcher();
                }
                else { //This means it is a blue square
                    label2.Visible = true;
                    foreach (PictureBox p in lst_pb)
                        p.BackgroundImage = null;
                }
            }
            else
            {
                if (o.Tag.ToString() != "12" && player == playerSelectingWrB(o)) 
                {
                    selSquare = o;
                    foreach (PictureBox p in lst_pb)
                        p.BackgroundImage = null;
                    int foundIndex = lst_pb.FindIndex(a => a == o);
                    lst_pb[foundIndex].BackgroundImage = Properties.Resources.blue_Back;
                    List<int[]> temp = new List<int[]>(Movement(foundIndex));
                    foreach (int[] item in temp)
                        lst_pb[coodToIndex(item)].BackgroundImage = Properties.Resources.Selected_Back;
                }
                else        //They clicked on an empty, not orange square
                {
                    label2.Visible = true;
                    foreach (PictureBox p in lst_pb)
                        p.BackgroundImage = null;
                }
            }
            if (player) {
                label1.Text = ("White's Turn");
                    }
            else
            {
                label1.Text = ("Black's Turn");
            }
        }
        private bool IsMate(PictureBox o)
        {
            int piece = lst_pb.FindIndex(a => a == o);
            for (int x = 0; x < 64; x++)
            {
                if (lst_pb[x].Tag.ToString() != "12")
                {
                    if (playerSelectingWrB(lst_pb[x]) == player && x != piece)
                    {
                        if (Movement(x).Any<int[]>())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }//Checks if a player can move anywhere
        private void DontCauseCheck(List<int[]> result, int pos)
        {
            string buffer;
            List<int[]> compare = new List<int[]>();

            foreach (int[] coords in result)
            {
                compare.Add(coords);
            }

            foreach (int[] coords in compare)
            {
                buffer = lst_pb[coodToIndex(coords)].Tag.ToString();
                lst_pb[coodToIndex(coords)].Tag = lst_pb[pos].Tag;
                lst_pb[pos].Tag = "12";
                if (IsCheck())
                {
                    lst_pb[pos].Tag = lst_pb[coodToIndex(coords)].Tag;
                    lst_pb[coodToIndex(coords)].Tag = buffer;
                    result.Remove(coords);
                }
                else
                {
                    lst_pb[pos].Tag = lst_pb[coodToIndex(coords)].Tag;
                    lst_pb[coodToIndex(coords)].Tag = buffer;
                }
            }
        } // Checks if moving a piece puts king in check and removes it if it does
        private PictureBox FindOtherKing()
        {
            for (int x = 0; x < 64; x++)
            {
                if (lst_pb[x].Tag.ToString() == "0" && player)
                {
                    return lst_pb[x];
                }
                if (lst_pb[x].Tag.ToString() == "1" && !player)
                {

                    return lst_pb[x];
                }
            }
            return null;
        }
        private List<int[]> Movement(int pos)
        {
            switch (int.Parse(lst_pb[pos].Tag.ToString()))
            {
                case 0://wKing
                case 1://bKing
                    return KingMovement(pos,false);
                case 2://wQueen
                case 3://bQueen
                    return QueenMovement(pos, false);
                case 4://wKnight
                case 5://bKnight
                    return KnightMovement(pos, false);
                case 6://wBishop
                case 7://bBishop
                    return BishopMovement(pos, false);
                case 8://wRook
                case 9://bRook
                    return RookMovement(pos, false);
                case 10://wPawn
                case 11://bPawn
                    return PawnMovement(pos, false);
                default:
                    return null;
            }
        }
        private bool IsCheck()
        {
            int king = 0;
            int pos = 0;
            if (!player)
            {
                //MessageBox.Show("blk Check");
                king = 1;
            }
            for (int x = 0; x < 64; x++)
            {
                if (lst_pb[x].Tag.ToString() == king.ToString())
                {
                    pos = x;
                    break;
                }
            }
            lst_pb[pos].Tag = 2 + king;
            List<int[]> temp = new List<int[]>(QueenMovement(pos, true));
            for (int j = 0; j < temp.Count; j++)
            {

                if (lst_pb[coodToIndex(temp[j])].Tag.ToString() == (3 - king).ToString())
                {
                    //MessageBox.Show("Queen Check");
                    lst_pb[pos].Tag = king;
                    return true;
                }
            }
            lst_pb[pos].Tag = 4 + king;
            temp = KnightMovement(pos, true);
            for (int j = 0; j < temp.Count; j++)
            {
                if (lst_pb[coodToIndex(temp[j])].Tag.ToString() == (5 - king).ToString())
                {
                    // MessageBox.Show("Knight Check");
                    lst_pb[pos].Tag = king;
                    return true;
                }
            }
            lst_pb[pos].Tag = 6 + king;
            temp = BishopMovement(pos, true);
            for (int j = 0; j < temp.Count; j++)
            {
                if (lst_pb[coodToIndex(temp[j])].Tag.ToString() == (7 - king).ToString())
                {
                    // MessageBox.Show("Bishop Check");
                    lst_pb[pos].Tag = king;
                    return true;
                }
            }
            lst_pb[pos].Tag = 8 + king;
            temp = RookMovement(pos, true);
            for (int j = 0; j < temp.Count; j++)
            {
                if (lst_pb[coodToIndex(temp[j])].Tag.ToString() == (9 - king).ToString())
                {
                    // MessageBox.Show("Rook Check");
                    lst_pb[pos].Tag = king;
                    return true;
                }
            }
            lst_pb[pos].Tag = 10 + king;
            temp = PawnMovement(pos, true);
            for (int j = 0; j < temp.Count; j++)
            {
                if (lst_pb[coodToIndex(temp[j])].Tag.ToString() == (11 - king).ToString())
                {
                    //MessageBox.Show("Pawn Check");
                    lst_pb[pos].Tag = king;
                    return true;
                }
            }
            lst_pb[pos].Tag = king;
            return false;
        } // Checks if king is in check
        private List<int[]> BishopMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            List<int[]> legalMove = new List<int[]>();
            int[] coords = GetCoords(pos);
            List<int[]> boundary = new List<int[]>() { null, null, null, null };
            for (int i = 1; i < 8; i++)
            {
                if (coords[0] + i < 8 && coords[1] + i < 8)
                {
                    int[] temp = { coords[0] + i, coords[1] + i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[1] == null)
                        boundary[1] = temp;
                    result.Add(temp);
                }
                if (coords[0] - i >= 0 && coords[1] - i >= 0)
                {
                    int[] temp = { coords[0] - i, coords[1] - i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[2] == null)
                        boundary[2] = temp;
                    result.Add(temp);
                }
                if (coords[0] - i >= 0 && coords[1] + i < 8)
                {
                    int[] temp = { coords[0] - i, coords[1] + i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[0] == null)
                        boundary[0] = temp;
                    result.Add(temp);
                }
                if (coords[0] + i < 8 && coords[1] - i >= 0)
                {
                    int[] temp = { coords[0] + i, coords[1] - i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[3] == null)
                        boundary[3] = temp;
                    result.Add(temp);
                }
            }
            legalMove.AddRange(result);
            for (int i = 0; i < result.Count; i++)
            {
                if (player == playerSelectingWrB(lst_pb[coodToIndex(result[i])]) && lst_pb[coodToIndex(result[i])].Tag.ToString() != "12")
                    legalMove.Remove(result[i]);
                else if (!IsBishopCoodInBoundary(boundary[0], boundary[1], boundary[2], boundary[3], result[i], coords))
                    legalMove.Remove(result[i]);
            }
            foreach (int[] item in boundary)
            {
                if (item != null && player != playerSelectingWrB(lst_pb[coodToIndex(item)]))
                    legalMove.Add(item);
            }
            if (!checking)
            {
                DontCauseCheck(legalMove, pos);
            }
            return legalMove;
        }
        private List<int[]> KnightMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            List<int[]> legalMove = new List<int[]>();
            int[] coords = GetCoords(pos);
            if (coords[0] - 2 >= 0)
            {
                if (coords[1] + 1 < 8)
                {
                    int[] temp = { coords[0] - 2, coords[1] + 1 };
                    result.Add(temp);
                }
                if (coords[1] - 1 >= 0)
                {
                    int[] temp = { coords[0] - 2, coords[1] - 1 };
                    result.Add(temp);
                }
            }
            if (coords[0] + 2 < 8)
            {
                if (coords[1] + 1 < 8)
                {
                    int[] temp = { coords[0] + 2, coords[1] + 1 };
                    result.Add(temp);
                }
                if (coords[1] - 1 >= 0)
                {
                    int[] temp = { coords[0] + 2, coords[1] - 1 };
                    result.Add(temp);
                }
            }
            if (coords[1] - 2 >= 0)
            {
                if (coords[0] + 1 < 8)
                {
                    int[] temp = { coords[0] + 1, coords[1] - 2 };
                    result.Add(temp);
                }
                if (coords[0] - 1 >= 0)
                {
                    int[] temp = { coords[0] - 1, coords[1] - 2 };
                    result.Add(temp);
                }
            }
            if (coords[1] + 2 < 8)
            {
                if (coords[0] + 1 < 8)
                {
                    int[] temp = { coords[0] + 1, coords[1] + 2 };
                    result.Add(temp);
                }
                if (coords[0] - 1 >= 0)
                {
                    int[] temp = { coords[0] - 1, coords[1] + 2 };
                    result.Add(temp);
                }
            }
            legalMove.AddRange(result);
            foreach (int[] item in result)
            {
                if (player == playerSelectingWrB(lst_pb[coodToIndex(item)]) && lst_pb[coodToIndex(item)].Tag.ToString() != "12")
                    legalMove.Remove(item);
            }
            if (!checking)
            {
                DontCauseCheck(legalMove, pos);
            }
            return legalMove;
        }
        private List<int[]> KingMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            List<int[]> legalMove = new List<int[]>();
            bool castleCheck = true;
            int[] coords = GetCoords(pos);
            if (coords[0] + 1 < 8)
            {
                int[] temp = { coords[0] + 1, coords[1] };
                result.Add(temp);
            }
            if (coords[0] + 1 < 8 && coords[1] + 1 < 8)
            {
                int[] temp = { coords[0] + 1, coords[1] + 1 };
                result.Add(temp);
            }
            if (coords[0] + 1 < 8 && coords[1] - 1 >= 0)
            {
                int[] temp = { coords[0] + 1, coords[1] - 1 };
                result.Add(temp);
            }
            if (coords[0] - 1 >= 0)
            {
                int[] temp = { coords[0] - 1, coords[1] };
                result.Add(temp);
            }
            if (coords[0] - 1 >= 0 && coords[1] + 1 < 8)
            {
                int[] temp = { coords[0] - 1, coords[1] + 1 };
                result.Add(temp);
            }
            if (coords[0] - 1 >= 0 && coords[1] - 1 >= 0)
            {
                int[] temp = { coords[0] - 1, coords[1] - 1 };
                result.Add(temp);
            }
            if (coords[1] + 1 < 8)
            {
                int[] temp = { coords[0], coords[1] + 1 };
                result.Add(temp);
            }
            if (coords[1] - 1 >= 0)
            {
                int[] temp = { coords[0], coords[1] - 1 };
                result.Add(temp);
            }
            legalMove.AddRange(result);
            foreach (int[] item in result)
            {
                if (player == playerSelectingWrB(lst_pb[coodToIndex(item)]) && lst_pb[coodToIndex(item)].Tag.ToString() != "12")
                    legalMove.Remove(item);
            }
            if (playerSelectingWrB(lst_pb[pos]))
            {
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ra1"))
                    {
                        castleCheck = false;
                    }
                }
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ke1"))
                    {
                        castleCheck = false;
                    }
                }
                if (castleCheck && lst_pb[1].Tag.ToString() == "12" && lst_pb[2].Tag.ToString() == "12" && lst_pb[3].Tag.ToString() == "12")
                {
                    List<int[]> path = new List<int[]>();
                    path.Add(new int[] { 2, 0 });
                    path.Add(new int[] { 3, 0 });
                    DontCauseCheck(path, pos);
                    if (path.Count > 1)
                    {
                        if (!IsCheck())
                        {
                            legalMove.Add(new int[] { 2, 0 });
                        }
                    }
                }
                castleCheck = true;
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Rh1"))
                    {
                        castleCheck = false;
                    }
                }
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ke1"))
                    {
                        castleCheck = false;
                    }
                }
                if (castleCheck && lst_pb[5].Tag.ToString() == "12" && lst_pb[6].Tag.ToString() == "12")
                {
                    List<int[]> path = new List<int[]>();
                    path.Add(new int[] { 5, 0 });
                    DontCauseCheck(path, pos);
                    if (path.Count > 0)
                    {
                        if (!IsCheck())
                        {
                            legalMove.Add(new int[] { 6, 0 });
                        }
                    }
                }
            }
            else
            {
                castleCheck = true;
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ra8"))
                    {
                        castleCheck = false;
                    }
                }
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ke8"))
                    {
                        castleCheck = false;
                    }
                }
                if (castleCheck && lst_pb[57].Tag.ToString() == "12" && lst_pb[58].Tag.ToString() == "12" && lst_pb[59].Tag.ToString() == "12")
                {
                    List<int[]> path = new List<int[]>();
                    path.Add(new int[] { 2, 7 });
                    path.Add(new int[] { 3, 7 });
                    DontCauseCheck(path, pos);
                    if (path.Count > 1)
                    {
                        if (!IsCheck())
                        {
                            legalMove.Add(new int[] { 2, 7 });
                        }
                    }
                }
                castleCheck = true;
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Rh8"))
                    {
                        castleCheck = false;
                    }
                }
                for (int x = 0; x < LstBx_MoveHistory.Items.Count; x++)
                {
                    if (LstBx_MoveHistory.Items[x].ToString().Contains("Ke8"))
                    {
                        castleCheck = false;
                    }
                }
                if (castleCheck && lst_pb[61].Tag.ToString() == "12" && lst_pb[62].Tag.ToString() == "12")
                {
                    List<int[]> path = new List<int[]>();
                    path.Add(new int[] { 5, 7 });
                    DontCauseCheck(path, pos);
                    if (path.Count > 0)
                    {
                        if (!IsCheck())
                        {
                            legalMove.Add(new int[] { 6, 7 });
                        }
                    }
                }
            }
            if (!checking)
            {
                player = !player;
                List<int[]> otherKing = KingMovement(lst_pb.FindIndex(a => a == FindOtherKing()), true);
                player = !player;
                List<int[]> buffer = new List<int[]>();

                foreach (int[] x in legalMove)
                {
                    buffer.Add(x);
                }
                foreach (int[] x in buffer)
                {
                    foreach (int[] y in otherKing)
                    {
                        if (y[0] == x[0])
                        {
                            if (y[1] == x[1])
                            {
                                legalMove.Remove(x);
                            }
                        }
                    }
                }
                DontCauseCheck(legalMove, pos);
            }
            return legalMove;
        }
        private List<int[]> RookMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            List<int[]> legalMove = new List<int[]>();
            List<int[]> boundary = new List<int[]>() { null, null, null, null };//[0]top;[1]bottom;[2]left;[3]right;
            int[] coords = GetCoords(pos);
            for (int i = 1; i < 8; i++)
            {
                if (coords[0] + i < 8)
                {
                    int[] temp = { coords[0] + i, coords[1] };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[3] == null)
                        boundary[3] = temp;
                    result.Add(temp);
                }
                if (coords[0] - i >= 0)
                {
                    int[] temp = { coords[0] - i, coords[1] };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[2] == null)
                        boundary[2] = temp;
                    result.Add(temp);
                }
                if (coords[1] + i < 8)
                {
                    int[] temp = { coords[0], coords[1] + i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[0] == null)
                        boundary[0] = temp;
                    result.Add(temp);
                }
                if (coords[1] - i >= 0)
                {
                    int[] temp = { coords[0], coords[1] - i };
                    if (lst_pb[coodToIndex(temp)].Tag.ToString() != "12" && boundary[1] == null)
                        boundary[1] = temp;
                    result.Add(temp);
                }
            }
            legalMove.AddRange(result);
            foreach (int[] item in result)
            {
                if (player == playerSelectingWrB(lst_pb[coodToIndex(item)]) && lst_pb[coodToIndex(item)].Tag.ToString() != "12") 
                    legalMove.Remove(item);
                else if (!IsRookCoodInBoundary(boundary, item))
                    legalMove.Remove(item);
            }
            foreach (int[] item in boundary)
            {
                if (item != null && player != playerSelectingWrB(lst_pb[coodToIndex(item)]))
                    legalMove.Add(item);
            }
            if (!checking)
            {
                DontCauseCheck(legalMove, pos);
            }
            return legalMove;
        }
        private List<int[]> QueenMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            int[] coords = GetCoords(pos);
            result.AddRange(BishopMovement(pos,true));
            result.AddRange(RookMovement(pos,true));
            if (!checking)
            {
                DontCauseCheck(result, pos);
            }
            return result;
        }
        private List<int[]> PawnMovement(int pos, bool checking)
        {
            List<int[]> result = new List<int[]>();
            List<int[]> legalMove = new List<int[]>();
            int[] coords = GetCoords(pos);
            if (playerSelectingWrB(lst_pb[coodToIndex(coords)]))
            {
                if (coords[1] == 1 && lst_pb[coodToIndex(new int[] { coords[0], coords[1] + 1 })].Tag.ToString() == "12")
                {
                    int[] temp = { coords[0], coords[1] + 1 };
                    result.Add(temp);
                    int[] temp2 = { coords[0], coords[1] + 2 };
                    result.Add(temp2);
                }
                else if (coords[1] + 1 < 8 && lst_pb[coodToIndex(new int[] { coords[0], coords[1] + 1 })].Tag.ToString() == "12") 
                {
                    int[] temp = { coords[0], coords[1] + 1 };
                    result.Add(temp);
                }
            }
            else
            {
                if (coords[1] == 6 && lst_pb[coodToIndex(new int[] { coords[0], coords[1] - 1 })].Tag.ToString() == "12")
                {
                    int[] temp = { coords[0], coords[1] - 1 };
                    result.Add(temp);
                    int[] temp2 = { coords[0], coords[1] - 2 };
                    result.Add(temp2);
                }
                else if (coords[1] - 1 >= 0 && lst_pb[coodToIndex(new int[] { coords[0], coords[1] - 1 })].Tag.ToString() == "12") 
                {
                    int[] temp = { coords[0], coords[1] - 1 };
                    result.Add(temp);
                }
            }
            legalMove.AddRange(result);
            foreach (int[] item in result)
            {
                if (lst_pb[coodToIndex(item)].Tag.ToString() != "12")
                    legalMove.Remove(item);
            }
            if (playerSelectingWrB(lst_pb[pos]))
            {
                if (coords[1] + 1 < 8)
                {
                    if (coords[0] + 1 < 8)
                        if (player != playerSelectingWrB(lst_pb[pos+9]) && lst_pb[pos + 9].Tag.ToString() != "12")
                            legalMove.Add(new int[] { coords[0] + 1, coords[1] + 1 });
                    if (coords[0] - 1 >= 0)
                        if (player != playerSelectingWrB(lst_pb[pos + 7]) && lst_pb[pos + 7].Tag.ToString() != "12")
                            legalMove.Add(new int[] { coords[0] - 1, coords[1] + 1 });
                }
            }
            else
            {
                if (coords[1] - 1 >= 0)
                {
                    if (coords[0] + 1 < 8)
                        if (player != playerSelectingWrB(lst_pb[coodToIndex(new int[] { coords[0] + 1, coords[1] - 1 })]) && lst_pb[coodToIndex(new int[] { coords[0] + 1, coords[1] - 1 })].Tag.ToString() != "12")
                            legalMove.Add(new int[] { coords[0] + 1, coords[1] - 1 });
                    if (coords[0] - 1 >= 0)
                        if (player != playerSelectingWrB(lst_pb[coodToIndex(new int[] { coords[0] - 1, coords[1] - 1 })]) && lst_pb[coodToIndex(new int[] { coords[0] - 1, coords[1] - 1 })].Tag.ToString() != "12")
                            legalMove.Add(new int[] { coords[0] - 1, coords[1] - 1 });
                }
            }
            if (specialMoves)
            {
                if (playerSelectingWrB(lst_pb[pos]))
                {
                    if (lst_pb[pos - 1] == enPassent)
                    {
                        legalMove.Add(new int[] { coords[0] - 1, coords[1] + 1 });
                    }
                    if (lst_pb[pos + 1] == enPassent)
                    {
                        legalMove.Add(new int[] { coords[0] + 1, coords[1] + 1 });
                    }
                }
                else
                {
                    if (lst_pb[pos - 1] == enPassent)
                    {
                        legalMove.Add(new int[] { coords[0] - 1, coords[1] - 1 });
                    }
                    if (lst_pb[pos + 1] == enPassent)
                    {
                        legalMove.Add(new int[] { coords[0] + 1, coords[1] - 1 });
                    }
                }
            }
            if (!checking)
            {
                DontCauseCheck(legalMove, pos);
            }
            return legalMove;
        }
        private bool IsBishopCoodInBoundary(int[] tl, int[] tr, int[] bl, int[] br, int[] pos, int[] coord)
        {
            bool tlf = true;
            bool trf = true;
            bool blf = true;
            bool brf = true;
            if (coord[0] > pos[0] && coord[1] < pos[1])
            {
                if (tl != null)
                {
                    if (pos[0] >= tl[0] && pos[1] <= tl[1])
                        tlf = true;
                    else
                        tlf = false;
                }
                else
                    tlf = true;
            }
            else if(coord[0] < pos[0]&&coord[1]<pos[1])
            {
                if (tr != null)
                {
                    if (pos[0] <= tr[0] && pos[1] <= tr[1])
                        trf = true;
                    else
                        trf = false;
                }
                else
                    trf = true;
            }
            else if(coord[0] > pos[0] && coord[1] > pos[1])
            {
                if (bl != null)
                {
                    if (pos[0] >= bl[0] && pos[1] >= bl[1])
                        blf = true;
                    else
                        blf = false;
                }
                else
                    blf = true;
            }
            else
            {
                if (br != null)
                {
                    if (pos[0] <= br[0] && pos[1] >= br[1])
                        brf = true;
                    else
                        brf = false;
                }
                else
                    brf = true;
            }
            if (tlf && trf && blf && brf)
                return true;
            else
                return false;
        }
        private bool IsRookCoodInBoundary(List<int[]> boundary, int[] pos)
        {
            bool top;
            bool bot;
            bool left;
            bool right;
            if (boundary[0] != null)
            {
                if (pos[1] <= boundary[0][1])
                    top = true;
                else
                    top = false;
            }
            else
                top = true;
            if (boundary[1] != null)
            {
                if (pos[1] >= boundary[1][1])
                    bot = true;
                else
                    bot = false;
            }
            else
                bot = true;
            if (boundary[2] != null)
            {
                if (pos[0] >= boundary[2][0])
                    left = true;
                else
                    left = false;
            }
            else
                left = true;
            if (boundary[3] != null)
            {
                if (pos[0] <= boundary[3][0])
                    right = true;
                else
                    right = false;
            }
            else
                right = true;
            if (top && bot && left && right)
                return true;
            else
                return false;
        }
        private bool playerSelectingWrB(PictureBox p)
        {
            if (p.Tag.ToString() != "12")
            {
                if (Convert.ToInt32(p.Tag) % 2 == 0)
                    return true;  //white
            }
            return false;  //black
        }                     ///// Use the tag of the picture box to find this piece is belong White/Black player
        private int[] GetCoords(int pos)
        {
            int[] coords = new int[2];
            coords[0] = pos % 8;
            coords[1] = pos / 8;
            return coords;
        }                                  ///// Convert the Index in "lst_pb" to Coordinate
        private int coodToIndex(int [] pos)
        {
            int value = pos[1] * 8 + pos[0];
            return pos[1] * 8 + pos[0];
        }                               ///// Convert the Coordinate to Index in "lst_pb"
        private void MovePieceViolent(bool sender)
        {
            if (sender == true) //Undo
            {
                MoveHistory temp = moveHistoriesTemp.Peek();
                lst_pb[temp.Start].Image = lst_pb[temp.End].Image;
                lst_pb[temp.Start].Tag = lst_pb[temp.End].Tag;
                if (!temp.EnPassentFlag)
                {
                    if (!temp.shamePiece.Empty())  //if no piece been taken
                    {
                        shamePiece tempShame = temp.shamePiece;
                        lst_pb[temp.End].Tag = tempShame.Tag;
                        lst_pb[temp.End].Image = GetImage(GetPiece(tempShame.Tag.ToString()), playerSelectingWrB(lst_pb[coodToIndex(tempShame.Coords)]), true);
                        HallOfTheShame(false, playerSelectingWrB(lst_pb[temp.End]), lst_pb[temp.End].Tag.ToString());
                    }
                    else  //else
                    {
                        lst_pb[temp.End].Image = null;
                        lst_pb[temp.End].Tag = "12";
                    }
                }
                else
                {
                    if (temp.shamePiece.Tag.ToString() == "11")
                    {
                        lst_pb[temp.End].Image = null;
                        lst_pb[temp.End].Tag = "12";
                        lst_pb[temp.End - 8].Tag = temp.shamePiece.Tag;
                        lst_pb[temp.End - 8].Image = GetImage(GetPiece(temp.shamePiece.Tag.ToString()), false, true);
                        HallOfTheShame(false, false, lst_pb[temp.End - 8].Tag.ToString());
                    }
                    else
                    {
                        lst_pb[temp.End].Image = null;
                        lst_pb[temp.End].Tag = "12";
                        lst_pb[temp.End + 8].Tag = temp.shamePiece.Tag;
                        lst_pb[temp.End + 8].Image = GetImage(GetPiece(temp.shamePiece.Tag.ToString()), true, true);
                        HallOfTheShame(false, true, lst_pb[temp.End + 8].Tag.ToString());
                    }
                }
                LstBx_MoveHistory.Items.Remove(temp.HistoryString);
                if (temp.PawnPromotionFlag == true)
                {
                    if (!playerSelectingWrB(lst_pb[temp.End]))
                    {
                        lst_pb[temp.Start].Image = Properties.Resources.Pieces_White_Pawn;
                        lst_pb[temp.Start].Tag = "10";
                    }
                    else
                    {
                        lst_pb[temp.Start].Image = Properties.Resources.Pieces_Black_Pawn;
                        lst_pb[temp.Start].Tag = "11";
                    }
                }
                if (temp.CastlingFlag)
                {
                    if (temp.CastlingWhere[0] && temp.CastlingWhere[1])
                    {
                        lst_pb[0].Image = Properties.Resources.Pieces_White_Rook;
                        lst_pb[0].Tag = "8";
                        lst_pb[3].Tag = "12";
                        lst_pb[3].Image = null;
                    }
                    else if(temp.CastlingWhere[0] && !temp.CastlingWhere[1])
                    {
                        lst_pb[7].Image = Properties.Resources.Pieces_White_Rook;
                        lst_pb[7].Tag = "8";
                        lst_pb[5].Tag = "12";
                        lst_pb[5].Image = null;
                    }
                    else if (!temp.CastlingWhere[0] && temp.CastlingWhere[1])
                    {
                        lst_pb[56].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[56].Tag = "9";
                        lst_pb[59].Image = null;
                        lst_pb[59].Tag = "12";
                    }
                    else
                    {
                        lst_pb[63].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[63].Tag = "9";
                        lst_pb[61].Image = null;
                        lst_pb[61].Tag = "12";
                    }
                }
                player = !player;
            }
            else  //Redo
            {
                MoveHistory temp = moveHistories.Peek();
                lst_pb[temp.End].Image = lst_pb[temp.Start].Image;
                lst_pb[temp.End].Tag = lst_pb[temp.Start].Tag;
                lst_pb[temp.Start].Image = null;
                lst_pb[temp.Start].Tag = "12";
                if (temp.EnPassentFlag)
                {
                    if (temp.shamePiece.Tag.ToString()=="11")
                    {
                        lst_pb[temp.End - 8].Tag = "12";
                        lst_pb[temp.End - 8].Image = null;
                    }
                    else
                    {
                        lst_pb[temp.End + 8].Tag = "12";
                        lst_pb[temp.End + 8].Image = null;
                    }
                }
                LstBx_MoveHistory.Items.Add(temp.HistoryString);
                if (!temp.shamePiece.Empty())
                    HallOfTheShame(true, playerSelectingWrB(lst_pb[temp.End]), temp.shamePiece.Tag.ToString());
                if (temp.PawnPromotionFlag)
                {
                    lst_pb[temp.End].Image = temp.PawnPromotionImage;
                    lst_pb[temp.End].Tag = temp.PawnPromotionTag;
                }
                if (temp.CastlingFlag)
                {
                    if (temp.CastlingWhere[0] && temp.CastlingWhere[1])
                    {
                        lst_pb[0].Image = null;
                        lst_pb[0].Tag = "12";
                        lst_pb[3].Tag = "8";
                        lst_pb[3].Image = Properties.Resources.Pieces_White_Rook;
                    }
                    else if (temp.CastlingWhere[0] && !temp.CastlingWhere[1])
                    {
                        lst_pb[7].Image = null;
                        lst_pb[7].Tag = "12";
                        lst_pb[5].Tag = "8";
                        lst_pb[5].Image = Properties.Resources.Pieces_White_Rook;
                    }
                    else if (!temp.CastlingWhere[0] && temp.CastlingWhere[1])
                    {
                        lst_pb[56].Image = null;
                        lst_pb[56].Tag = "12";
                        lst_pb[59].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[59].Tag = "9";
                    }
                    else
                    {
                        lst_pb[63].Image = null;
                        lst_pb[63].Tag = "12";
                        lst_pb[61].Image = Properties.Resources.Pieces_Black_Rook;
                        lst_pb[61].Tag = "9";
                    }
                }
                player = !player;
            }
            foreach (PictureBox item in lst_pb)
                item.BackgroundImage = null;
        }                        ///// Use for Undo/Redo Button
        private Image GetImage(char Piece, bool WrB,bool size)
        {
            if (size == true)
            {
                switch (Piece)
                {
                    case 'K':
                        if (WrB)
                            return Properties.Resources.Pieces_White_King;
                        else
                            return Properties.Resources.Pieces_Black_King;
                    case 'Q':
                        if (WrB)
                            return Properties.Resources.Pieces_White_Queen;
                        else
                            return Properties.Resources.Pieces_Black_Queen;
                    case 'N':
                        if (WrB)
                            return Properties.Resources.Pieces_White_Knight;
                        else
                            return Properties.Resources.Pieces_Black_Knight;
                    case 'B':
                        if (WrB)
                            return Properties.Resources.Pieces_White_Bishop;
                        else
                            return Properties.Resources.Pieces_Black_Bishop;
                    case 'R':
                        if (WrB)
                            return Properties.Resources.Pieces_White_Rook;
                        else
                            return Properties.Resources.Pieces_Black_Rook;
                    case 'P':
                        if (WrB)
                            return Properties.Resources.Pieces_White_Pawn;
                        else
                            return Properties.Resources.Pieces_Black_Pawn;
                    default:
                        return null;
                }
            }
            else
            {
                switch (Piece)
                {
                    case 'K':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_king;
                        else
                            return Properties.Resources.Small_black_chess_king;
                    case 'Q':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_queen;
                        else
                            return Properties.Resources.Small_black_chess_queen;
                    case 'N':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_knight;
                        else
                            return Properties.Resources.Small_black_chess_knight;
                    case 'B':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_bishop;
                        else
                            return Properties.Resources.Small_black_chess_bishop;
                    case 'R':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_rook;
                        else
                            return Properties.Resources.Small_black_chess_bishop;
                    case 'P':
                        if (WrB)
                            return Properties.Resources.Small_white_chess_pawn;
                        else
                            return Properties.Resources.Small_black_chess_pawn;
                    default:
                        return null;
                }
            }
        }            ///// Return small/large pieces pic
        private void HallOfTheShame(bool show, bool player, string tag)
        {
            if (show == true)  //show
            {
                if (player == true)
                {
                    lstBlackPoint[blackCounter].Image = GetImage(GetPiece(tag), false, false);
                    blackCounter++;
                    WhitePointCounter.Text= (int.Parse(WhitePointCounter.Text) + PointCalculator(true, GetPiece(tag))).ToString();
                }
                else
                {
                    lstWhitePoint[whiteCounter].Image = GetImage(GetPiece(tag), true, false);
                    whiteCounter++;
                    BlackPointCounter.Text = (int.Parse(BlackPointCounter.Text) + PointCalculator(true, GetPiece(tag))).ToString();
                }
            }
            else  //delete
            {
                if (player == true)
                {
                    whiteCounter--;
                    lstWhitePoint[whiteCounter].Image = null;
                    BlackPointCounter.Text = (int.Parse(BlackPointCounter.Text) + PointCalculator(false, GetPiece(tag))).ToString();
                }
                else
                {
                    blackCounter--;
                    lstBlackPoint[blackCounter].Image = null;
                    WhitePointCounter.Text = (int.Parse(WhitePointCounter.Text) + PointCalculator(false, GetPiece(tag))).ToString();
                }
            }
        }   ///// Add/Remove pieces pic from the display
        private int PointCalculator(bool Operator, char tag)
        {
            if (Operator)
            {
                switch (tag)
                {
                    case 'P':
                        return 1;
                    case 'B':
                    case 'N':
                        return 3;
                    case 'Q':
                        return 9;
                    case 'R':
                        return 5;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (tag)
                {
                    case 'P':
                        return -1;
                    case 'B':
                    case 'N':
                        return -3;
                    case 'Q':
                        return -9;
                    case 'R':
                        return -5;
                    default:
                        return 0;
                }
            }
        }
        private void TickSwitcher()
        {
            if (player == true)
            {
                wtTimer.Start();
                bkTimer.Stop();
            }
            else
            {
                wtTimer.Stop();
                bkTimer.Start();
            }
        }                                       ///// The timer switch between white and black
        private bool ThreefoldRepetition()
        {
            if (moveHistories.Count >= 6)
            {
                MoveHistory[] array = new MoveHistory[moveHistories.Count];
                moveHistories.CopyTo(array,0);
                if ((array[0].Start == array[2].End && array[2].End == array[4].Start) && (array[0].End == array[2].Start && array[2].Start == array[4].End))
                    if ((array[1].Start == array[3].End && array[3].End == array[5].Start) && (array[1].End == array[3].Start && array[3].Start == array[5].End))
                        return true;
            }
            return false;
        }                                ///// Check Threefold Repetition

        /////Tick
        private void wtTimer_Tick(object sender, EventArgs e)
        {
            if (TimeSpan.Zero != whiteTime)
            {
                string str = string.Format("{0:D2}:{1:D2}", whiteTime.Minutes, whiteTime.Seconds);
                wTimerDisplay.Text = str;
                whiteTime = whiteTime.Subtract(new TimeSpan(0, 0, 1));
            }
            else
            {
                wtTimer.Stop();
                if (CustomMsg.Show("White Player time out, Black Player Win") == DialogResult.OK)
                {
                    SetDefaultValues();
                    blackWin++;
                    BlackWinCounter.Text = blackWin.ToString();
                }
            }
        }
        private void bkTimer_Tick(object sender, EventArgs e)
        {
            if (TimeSpan.Zero != blackTime)
            {
                string str = string.Format("{0:D2}:{1:D2}", blackTime.Minutes, blackTime.Seconds);
                bkTimerDisplay.Text = str;
                blackTime = blackTime.Subtract(new TimeSpan(0, 0, 1));
            }
            else
            {
                bkTimer.Stop();
                if (CustomMsg.Show("Black Player time out, White Player Win") == DialogResult.OK)
                {
                    SetDefaultValues();
                    whiteWin++;
                    WhiteWinCounter.Text = whiteWin.ToString();
                }
            }
        }

        /////Function Button Click
        private void Undobtn_Click(object sender, EventArgs e)
        {
            moveHistoriesTemp.Push(moveHistories.Pop());
            if (!moveHistories.Any())
                btnUndo.Enabled = false;
            MovePieceViolent(true);
            btnRedo.Enabled = true; 
            if(IsCheck())
                FindOtherKing().BackgroundImage = Properties.Resources.red_Back;
            if (player)
            {
                label1.Text = ("White's Turn");
            }
            else
            {
                label1.Text = ("Black's Turn");
            }
            label2.Visible = false;
            TickSwitcher();
        }
        private void btnRedo_Click(object sender, EventArgs e)
        {
            moveHistories.Push(moveHistoriesTemp.Pop());
            if (!moveHistoriesTemp.Any())
                btnRedo.Enabled = false;
            MovePieceViolent(false);
            btnUndo.Enabled = true;
            if (IsCheck())
                FindOtherKing().BackgroundImage = Properties.Resources.red_Back;
            if (player)
            {
                label1.Text = ("White's Turn");
            }
            else
            {
                label1.Text = ("Black's Turn");
            }
            label2.Visible = false;
            TickSwitcher();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnReStart_Click(object sender, EventArgs e)
        {
            SetDefaultValues();
        }

        /////Picture Box Click
        private void a1_Click(object sender, EventArgs e)
        {
            Click_event(a1);
        }
        private void a2_Click(object sender, EventArgs e)
        {
            Click_event(a2);
        }
        private void a3_Click(object sender, EventArgs e)
        {
            Click_event(a3);
        }
        private void a4_Click(object sender, EventArgs e)
        {
            Click_event(a4);
        }
        private void a5_Click(object sender, EventArgs e)
        {
            Click_event(a5);
        }
        private void a6_Click(object sender, EventArgs e)
        {
            Click_event(a6);
        }
        private void a7_Click(object sender, EventArgs e)
        {
            Click_event(a7);
        }
        private void a8_Click(object sender, EventArgs e)
        {
            Click_event(a8);
            
        }
        private void b1_Click(object sender, EventArgs e)
        {
            Click_event(b1);
        }
        private void b2_Click(object sender, EventArgs e)
        {
            Click_event(b2);
        }
        private void b3_Click(object sender, EventArgs e)
        {
            Click_event(b3);
        }
        private void b4_Click(object sender, EventArgs e)
        {
            Click_event(b4);
        }
        private void b5_Click(object sender, EventArgs e)
        {
            Click_event(b5);
        }
        private void b6_Click(object sender, EventArgs e)
        {
            Click_event(b6);
        }
        private void b7_Click(object sender, EventArgs e)
        {
            Click_event(b7);
        }
        private void b8_Click(object sender, EventArgs e)
        {
            Click_event(b8);
        }
        private void c1_Click(object sender, EventArgs e)
        {
            Click_event(c1);
        }
        private void c2_Click(object sender, EventArgs e)
        {
            Click_event(c2);
        }
        private void c3_Click(object sender, EventArgs e)
        {
            Click_event(c3);
        }
        private void c4_Click(object sender, EventArgs e)
        {
            Click_event(c4);
        }
        private void c5_Click(object sender, EventArgs e)
        {
            Click_event(c5);
        }
        private void c6_Click(object sender, EventArgs e)
        {
            Click_event(c6);
        }
        private void c7_Click(object sender, EventArgs e)
        {
            Click_event(c7);
        }
        private void c8_Click(object sender, EventArgs e)
        {
            Click_event(c8);
        }
        private void d1_Click(object sender, EventArgs e)
        {
            Click_event(d1);
        }
        private void d2_Click(object sender, EventArgs e)
        {
            Click_event(d2);
        }
        private void d3_Click(object sender, EventArgs e)
        {
            Click_event(d3);
        }
        private void d4_Click(object sender, EventArgs e)
        {
            Click_event(d4);
        }
        private void d5_Click(object sender, EventArgs e)
        {
            Click_event(d5);
        }
        private void d6_Click(object sender, EventArgs e)
        {
            Click_event(d6);
        }
        private void d7_Click(object sender, EventArgs e)
        {
            Click_event(d7);
        }
        private void d8_Click(object sender, EventArgs e)
        {
            Click_event(d8);
        }
        private void e1_Click(object sender, EventArgs e)
        {
            Click_event(e1);
        }
        private void e2_Click(object sender, EventArgs e)
        {
            Click_event(e2);
        }
        private void e3_Click(object sender, EventArgs e)
        {
            Click_event(e3);
        }
        private void e4_Click(object sender, EventArgs e)
        {
            Click_event(e4);
        }
        private void e5_Click(object sender, EventArgs e)
        {
            Click_event(e5);
        }
        private void e6_Click(object sender, EventArgs e)
        {
            Click_event(e6);
        }
        private void e7_Click(object sender, EventArgs e)
        {
            Click_event(e7);
        }
        private void e8_Click(object sender, EventArgs e)
        {
            Click_event(e8);
        }
        private void f1_Click(object sender, EventArgs e)
        {
            Click_event(f1);
        }
        private void f2_Click(object sender, EventArgs e)
        {
            Click_event(f2);
        }
        private void f3_Click(object sender, EventArgs e)
        {
            Click_event(f3);
        }
        private void f4_Click(object sender, EventArgs e)
        {
            Click_event(f4);
        }
        private void f5_Click(object sender, EventArgs e)
        {
            Click_event(f5);
        }
        private void f6_Click(object sender, EventArgs e)
        {
            Click_event(f6);
        }
        private void f7_Click(object sender, EventArgs e)
        {
            Click_event(f7);
        }
        private void f8_Click(object sender, EventArgs e)
        {
            Click_event(f8);
        }
        private void g1_Click(object sender, EventArgs e)
        {
            Click_event(g1);
        }
        private void g2_Click(object sender, EventArgs e)
        {
            Click_event(g2);
        }
        private void g3_Click(object sender, EventArgs e)
        {
            Click_event(g3);
        }
        private void g4_Click(object sender, EventArgs e)
        {
            Click_event(g4);
        }
        private void g5_Click(object sender, EventArgs e)
        {
            Click_event(g5);
        }
        private void g6_Click(object sender, EventArgs e)
        {
            Click_event(g6);
        }
        private void g7_Click(object sender, EventArgs e)
        {
            Click_event(g7);
        }
        private void g8_Click(object sender, EventArgs e)
        {
            Click_event(g8);
        }
        private void h1_Click(object sender, EventArgs e)
        {
            Click_event(h1);
        }
        private void h2_Click(object sender, EventArgs e)
        {
            Click_event(h2);
        }
        private void h3_Click(object sender, EventArgs e)
        {
            Click_event(h3);
        }
        private void h4_Click(object sender, EventArgs e)
        {
            Click_event(h4);
        }
        private void h5_Click(object sender, EventArgs e)
        {
            Click_event(h5);
        }
        private void h6_Click(object sender, EventArgs e)
        {
            Click_event(h6);
        }
        private void h7_Click(object sender, EventArgs e)
        {
            Click_event(h7);
        }
        private void h8_Click(object sender, EventArgs e)
        {
            Click_event(h8);
        }
        
        public struct shamePiece
        {
            private int[] _coords;
            private string _tag;
            public shamePiece(int[] _coords, string _tag)
            {
                this._coords = _coords;
                this._tag = _tag;
            }
            public int[] Coords
            {
                get { return _coords; }
            }
            public string Tag
            {
                get { return _tag; }
            }
            public bool Empty()
            {
                if (_coords == null)
                    return true;
                return false;
            }
        }

        public class MoveHistory
        {
            private int _start;
            private int _end;
            private string _historyString;
            private string _startTag;
            private string _endTag;

            public MoveHistory(int start, int end, string startTag, string endTag)
            {
                _start = start;
                _end = end;
                //_takePiece = takePiece;
                _startTag = startTag;
                _endTag = endTag;
                _historyString = string.Empty;
            }
            public void HistoryStringWriter(char startPiece, string startName, char endPiece, string endName)
            {
                string movHist = string.Empty;
                movHist += startPiece + startName;
                if (!this.shamePiece.Empty())
                    movHist += "x" + endPiece;
                else
                    movHist += "-";
                movHist += endName;
                _historyString = movHist;
                _historyString = movHist;
            }
            public int Start { get { return _start; } }
            public int End { get { return _end; } }
            public string HistoryString { get { return _historyString; } }
            public shamePiece shamePiece { get; set; }
            public bool PawnPromotionFlag { get; set; } = false;
            public string PawnPromotionTag { get; set; } = string.Empty;
            public Image PawnPromotionImage { get; set; } = null;
            public bool EnPassentFlag { get; set; } = false;
            public bool CastlingFlag { get; set; } = false;
            public bool[] CastlingWhere { get; set; } = null;//[0]:w/b;[1]:l/r
        }
    }
}
