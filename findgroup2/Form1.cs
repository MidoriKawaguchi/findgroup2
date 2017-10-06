using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace findgroup2
{
    public partial class Form1 : Form
    {
        string[] outputfiles;
        int NumberOfData = 108;
        public Form1()
        {
            InitializeComponent();
        }

        private void fileSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = true;
            o.Filter = "CSV|*.csv";
            if (o.ShowDialog() == DialogResult.OK)
            {
                outputfiles = o.FileNames;
                MessageBox.Show("Select" + outputfiles.Length + "files");
                //textBox2.Text = Path.GetDirectoryName(outputfiles[0]);
            }
            else
            {
                MessageBox.Show("Open file Error");
            }

        }

        public void patternCompare_Click(object sender, EventArgs e)
        {
            float frequency = 1000;
            float T = 1 / frequency;
            //windowsizeは適宜変える
            int windowsize = 100;
            string[] dataInOneLine;
            string[] csvDataArray;
            int[][] label = new int[outputfiles.Length][];
            int[] csvDataLength = new int[outputfiles.Length];
            double[][] timestamp = new double[outputfiles.Length][]; // for pick up timestamp

            int[] fileNumber = new int[outputfiles.Length];
            string csvFileOutput = "filenumber, label \r\n";
            string newfile1 = Path.GetDirectoryName(outputfiles[0]) + "labeloutput_test.csv";
            string newfile2 = Path.GetDirectoryName(outputfiles[0]) + "patternlabel_test.csv";

            //Dictionary<int, int[]> foundPattern = new Dictionary<int, int[]>();



            for (int j = 0; j < outputfiles.Length; j++)//outputfiles.Lengths:number of chosen files 
            {
                string file = outputfiles[j];           //file turn to 1 line csvDataArray
                //Console.WriteLine("File Num: {0}", Path.GetFileNameWithoutExtension(outputfiles[j]).Substring(19, 3));
                //fileName[j] = ((Path.GetFileNameWithoutExtension(outputfiles[j])[19] + Path.GetFileNameWithoutExtension(outputfiles[j])[20] + Path.GetFileNameWithoutExtension(outputfiles[j])[21]));
                fileNumber[j] = int.Parse(Path.GetFileNameWithoutExtension(outputfiles[j]).Substring(19, 3));
                dataInOneLine = null;
                dataInOneLine = File.ReadAllLines(file);        //read all data as 1 line
                label[j] = new int[dataInOneLine.Length / windowsize + 1];
                timestamp[j] = new double[dataInOneLine.Length / windowsize + 1];       //for timestamp
                csvDataLength[j] = dataInOneLine.Length;

                csvFileOutput = csvFileOutput + fileNumber[j];

                for (int i = 1, k = 0; i < dataInOneLine.Length; i = i + windowsize, k++) //データ取得
                {
                    csvDataArray = dataInOneLine[i].Split(',');

                    //get label
                    label[j][k] = int.Parse(csvDataArray[5]);           //ラベル
                    timestamp[j][k] = double.Parse(csvDataArray[0]);    //timestamp

                    csvFileOutput = csvFileOutput + "," + label[j][k] + "," + timestamp[j][k] ;



                }
                csvFileOutput = csvFileOutput + "\r\n";
                
            }
            File.WriteAllText(newfile1, csvFileOutput);


            Dictionary<int[], patternData> foundPattern = new Dictionary<int[], patternData>(new PatternEqual());
            //Dictionary<int[], patternData> foundPattern = new Dictionary<int[], patternData>(new PatternComp());
            int Exist = 1;
            int NotExist = 0;

            int flagForExist = NotExist;                                       //重複防止のためのフラグ　★追加項目

            for (int numberOfFile = 0; numberOfFile < label.Length; ++numberOfFile)    //ファイルを見ていく label.Length: numbers of files
            {
                int[] curLabel = label[numberOfFile];                            //ファイル［numberOfFile］番目のラベル系列
                int curFileNumber = fileNumber[numberOfFile];                    //ファイル番号
                double[] curTimestamp = timestamp[numberOfFile];                 //for timestamp
                for (int start = 0, end = 1; start < curLabel.Length;)  //ラベルのまとまりを比較していく start:見始め　end:見終わり
                {
                    
                    int[] labelStored = new int[end - start];                   //ラベルパターン格納用(labelStored)
                    double[] timestampStored = new double[end - start];
                    for (int i = 0 ,j = 0; i < (end - start); ++i)
                    {
                        if (curLabel[start + i] != -1)                  //-1はラベルに含めない　★追加項目
                        {
                            if (j == 0 || curLabel[start + i] != labelStored[j - 1])   //重なるラベルも統合
                            {
                                labelStored[j] = curLabel[start + i];                    //ラベルパターン:ファイル［start + i]］番目のラベル系列
                                timestampStored[j] = curTimestamp[start + i];
                                
                                ++j;
                            }
                        }
                        //Console.WriteLine("labelStored[" + i + "]:" + labelStored[i]);
                    }
                    if (foundPattern.ContainsKey(labelStored))                  //ラベルパターンがDictionaryにあるとき
                    {
                        end++;
                        flagForExist = Exist;                                       //重複防止のためのflag ★追加項目
                        Console.WriteLine("end++");
                    }
                    else
                    {                                                   //これまでのラベルパターンがDictonaryにないとき  ながさを伸ばして比較
                        int labelCountOfPattern = 1;
                        var tmpFileNameList = new List<int>();
                        //var tmpTimestampList = new List<double>();
                        //if (flagForExsit == 1 && line > 1)                      //★追加 flagによりこれまでend++をしたか判断
                        //{
                        //    //Console.WriteLine("flagForExsit == 1");
                        //    //Console.WriteLine("list:" + tmpFileNameList.ToArray().ToString());

                        //   　//if (fileName[line].ToString().Equals(fileName[line-1].ToString()))           //★追加　end++ したのはひとつ前のファイル。そのファイルが含むファイル番号が一致するならば、
                        //    //{
                        //    //    Remove();     //★追加　直前のパターンのファイル番号を消し
                        //    //    tmpFileNameList.Add(curName);                 //★追加　現在のファイル番号を残す
                        //    //    Console.WriteLine("this is for check");
                        //    //}
                        //}
                        //else
                        //{
                        tmpFileNameList.Add(curFileNumber);
                        
                        //}

                      

                        for (int searchNumberOfFile = numberOfFile + 1; searchNumberOfFile < label.Length; ++searchNumberOfFile)
                        {                                                   //searchNumberOfFile:比較するlabelの番号
                            int[] cmpLabel = label[searchNumberOfFile];     //cmpLabel:比較するもの　ファイル［searchNumberOfFile］番目のラベル系列
                            for (int cs = 0; cs < cmpLabel.Length;)         //cs: cs+position :比較されるもの　posをいじることで値変化
                            {
                                int position = 0;
                                for (; position < labelStored.Length && cs + position < cmpLabel.Length && labelStored[position] == cmpLabel[cs + position]; ++position) { }
                                //position(位置)はラベルパターンの長さ以下
                                //比較対象の長さ以下
                                //比較対象とラベルパターンが一緒
                                //上記の条件が揃うとき、位置を移動(左に1つ)
                                //Console.WriteLine("labelpattern" + labelStored[position] + " \t" + cmpLabel[cs + position]);

                                if (position == labelStored.Length)                 //長さが一緒なら一致
                                {
                                    labelCountOfPattern++;
                                    tmpFileNameList.Add(fileNumber[searchNumberOfFile]);
                                    //cs += position;
                                    break;
                                }
                                else
                                {
                                    cs++;
                                    flagForExist = NotExist; //★　追加
                                }
                            }
                        }
                        if (labelCountOfPattern > 1 && flagForExist == Exist)         //&& flagForExsit != 1 ★追加項目　重複防止
                        {
                            foundPattern[labelStored] = new patternData 
                            {
                                countOfPattern = labelCountOfPattern,
                                fileNumberOfPattern = tmpFileNameList
                            };
                            end++;
                            flagForExist = NotExist;                                       //重複防止のためのflag ★追加項目
                        }
                        else
                        {
                            end = curLabel.Length + 1;
                            // Console.WriteLine("curLabel.Length :" + curLabel.Length);
                        }
                    }
                    

                    if (end > curLabel.Length - 1)
                    {
                        start++;
                        end = start + 1;
                        //Console.WriteLine("new start:" + start);
                        //Console.WriteLine("new end:" + end);
                    }

                }
            }

            Console.WriteLine("classfiy finished");

        
            
            //int filternumber = 0;   //重複するファイル用にフィルターをかける

            string csvFileOutput2 = "labelpattern,count \r\n";
           

            //重複するファイル削除用
            //DuplicationDictionary<int[], patternData> cutfiltered = new DuplicationDictionary<int[], patternData>(new DuplicationCut());
            //foreach (KeyValuePair<int[], patternData> pattern in foundPattern)
            //{

            //    //重複しているファイルと系列のセットは、最も長い系列を残しあとは削除

            //    //該当ファイルの数が同じものを集め配列として格納
            //    //ファイルの名前が同じ＆＆含まれているのであれば
            //    //ラベル系列の長さがもっとも長いものを選ぶ
                

            //    for (int i = 0; i < pattern.Key.Length; ++i)
            //    {
            //        csvFileOutput2 += pattern.Key[i] + " "; //ラベルの配列を出力
            //        Console.WriteLine("patten.Key :" + pattern.Key[i]);
            //    }
            //    csvFileOutput2 += "," + pattern.Value.countOfPattern + "," + string.Join(" ", pattern.Value.fileNumberOfPattern.Select(v => v.ToString())) + "\r\n";
            //    //patternoutput += "," + pattern.Value.timestampOfPattern + "\r\n";
            //}
            ////File.WriteAllText(newfile2, patternoutput);

            //csvFileOutput2 += "\r\n";


            //以下にてめぼしをつけて、異なるパターンを探す
            SortedDictionary<int[], patternData> filtered = new SortedDictionary<int[], patternData>(new PatternComp());
            Console.WriteLine("test" + filtered.Count());
            foreach (KeyValuePair<int[], patternData> pattern in foundPattern)
            {
                if (pattern.Value.countOfPattern > NumberOfData*0.1 && pattern.Value.countOfPattern < NumberOfData*0.4 && pattern.Key.Length > 5)//該当するファイル数が入力ファイル数の3％以上40%以下 && labelLength is more than 5 
                {
                    filtered.Add(pattern.Key, pattern.Value);//pattern.Key:label  pattern.Value:file number
                }

            }



            Console.WriteLine("test test");

            
          
            foreach (KeyValuePair<int[], patternData> pattern in filtered)//出力用
            {
                for (int i = 0; i < pattern.Key.Length; ++i)
                {
                    csvFileOutput2 += pattern.Key[i] + " ";//pattern.Key:ラベル系列の配列,1つのパターン
                   
                }
                csvFileOutput2 += "," + pattern.Value.countOfPattern + "," + string.Join(" ", pattern.Value.fileNumberOfPattern.Select(v => v.ToString())) + "\r\n";
                //+ "," + pattern.Value.timestampOfPattern +"\r\n";
            }
            csvFileOutput2 += "\r\n";
            Console.WriteLine("test test2");



            List<int[]> comparedFile = new List<int[]>();
            foreach (var fileList1 in filtered)
            {
                comparedFile.Add(fileList1.Key);
                foreach (var fileList2 in filtered)
                {
                    if (!comparedFile.Contains(fileList2.Key))
                    {
                        var intersectList = fileList1.Value.fileNumberOfPattern.Intersect(fileList2.Value.fileNumberOfPattern);
                        
                        if ((fileList1.Key[0] == fileList2.Key[0]) && (fileList1.Key.Length == fileList2.Key.Length)&& intersectList.Count() == 0  )
                            //先頭のラベルがおなじ　&&　ラベル配列の長さも同じ
                            //intersectList.Count() > 1 && intersectList.Count() < 3
                            //積集合(同じラベルがでてくる）の個数が1より大きく３未満
                        {
                            csvFileOutput2 += "interectList \r\n";

                            foreach (var v in fileList1.Key) csvFileOutput2 += v.ToString() + " ";
                            csvFileOutput2 += "| ";
                            foreach (var v in fileList2.Key) csvFileOutput2 += v.ToString() + " ";

                            csvFileOutput2 += "\r\n";

                            //foreach (var v in fileList1.Value.timestamp) patternoutput += v.ToString() + " ";
                            csvFileOutput2 += "\r\n";
                        }
                    }
                }
            }


            File.WriteAllText(newfile2, csvFileOutput2);

            MessageBox.Show("Finished");

        }

        public static string PrintArray<T>(T[] a)
        {
            string s = a[0].ToString();
            for (int i = 1; i < a.Length; i++)
            {
                s += ", " + a[i].ToString();
            }
            return s;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
    class PatternEqual : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; ++i)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }
        public int GetHashCode(int[] x)
        {
            int rv = 0;
            for (int i = 0; i < 4 && i < x.Length; ++i) //countが以上
            {
                rv += x[i] << (i * 6);
            }
            rv += x.Length;
            return rv;
        }
    };
    class PatternComp : IComparer<int[]>
    {
        public int Compare(int[] x, int[] y)
        {
            for (int i = 0; i < x.Length && i < y.Length; ++i)
            {
                int diff = x[i] - y[i];
                if (diff != 0) return diff;
            }
            return x.Length - y.Length;
        }
    };
    public struct patternData
    {
        public int countOfPattern;
        public List<int> fileNumberOfPattern;
        //public List<double> timestampOfPattern;      //Add timestamp
    };




}
