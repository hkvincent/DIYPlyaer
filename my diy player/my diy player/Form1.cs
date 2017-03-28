using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace my_diy_player {
    public partial class Form1 : Form {
        //save path of the media
        List<string> listPath = new List<string>();
        //save the extension file
        List<String> extensionList = new List<string>() {
          ".mp3",".wav",".mp4",".avi",".wmv",".mkv"
        };
        //get application path
        string str = System.Windows.Forms.Application.StartupPath + "\\read.txt";
        string process = System.Windows.Forms.Application.StartupPath + "\\process.txt";
        //list for storing process
        List<string> listProcess = new List<string>();
        string currentname;

        public Form1() {
            InitializeComponent();
        }


        /// <summary>
        /// hide the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender,EventArgs e) {
            panel1.Visible = false;
            panel2.Visible = true;
            this.player.Size = new System.Drawing.Size(this.ClientSize.Width - 50,this.ClientSize.Height);

        }


        /// <summary>
        /// when loading form1 read the listbox file and process file for breakpoint continue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender,EventArgs e) {

            panel2.Visible = false;
            timer1.Enabled = false;
            //Directory.CreateDirectory(process);
            try {
                String[] strpath = File.ReadAllLines(str,Encoding.Default);
                if(strpath != null && strpath.Length > 1) {
                    for(int i = 0; i < strpath.Length; i++) {
                        listPath.Add(strpath[i]);
                        listBox1.Items.Add(Path.GetFileName(strpath[i]));
                    }
                }
                string[] processPosition = File.ReadAllLines(process,Encoding.Default);
                if(processPosition.Length > 0)
                    listProcess = processPosition.ToList();
            }
            catch {

            }
        }


        /// <summary>
        /// show the listbox and hide the show button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender,EventArgs e) {
            this.player.Size = new System.Drawing.Size(this.ClientSize.Width - 200,this.ClientSize.Height);
            panel2.Visible = false;
            panel1.Visible = true;

        }


        /// <summary>
        /// for importing file,when import file the file will add tothe listpath list and listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender,EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MP3|*.mp3|mp4|*.mp4|avi|*.avi|miusc|*.wav|ALL|*.*";
            ofd.Title = "choose your file";
            ofd.Multiselect = true;
            ofd.ShowDialog();

            //get the file path of  OpenFileDialog user has selected.
            string[] path = ofd.FileNames;
            for(int i = 0; i < path.Length; i++) {
                if(!listPath.Contains(path[i])) {
                    //put the full path of selected file into generic list
                    listPath.Add(path[i]);
                    //and insert the file name to the listbox and show to user
                    listBox1.Items.Add(Path.GetFileName(path[i]));
                    File.AppendAllText(str,path[i] + "\r\n",Encoding.Default);
                }
            }
        }





        /// <summary>
        ///  double click listbox to play the seleceted item and load the breakpoint of last time playing
        ///  of this item and will start  on this breakpoint position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_DoubleClick(object sender,EventArgs e) {
            if(listBox1.Items.Count == 0) {
                MessageBox.Show("please select file you want to play");
                return;
            }
            try {
                player.URL = listPath[listBox1.SelectedIndex];
                player.Ctlcontrols.play();

                //string name = Path.GetFileName(listPath[listBox1.SelectedIndex]);
                // DirectoryInfo dir = new DirectoryInfo(process);
                //FileInfo[] fileInfo=dir.GetFiles();

                currentname = Path.GetFileName(listPath[listBox1.SelectedIndex]);

                string[] processPosition = File.ReadAllLines(process,Encoding.Default);
                if(processPosition.Length > 0)
                    listProcess = processPosition.ToList();
                foreach(var item in listProcess) {
                    string[] strnew = item.Split(new char[] { '+' },StringSplitOptions.RemoveEmptyEntries);
                    if(strnew[0].Equals(currentname)) {
                        player.Ctlcontrols.currentPosition = Double.Parse(strnew[1]);
                        currentname = strnew[0];
                        listProcess.Remove(item);
                        return;
                    }
                }
                // lblInformation.Text = musicPlayer.currentMedia.duration.ToString();
            }
            catch { }
        }


        /// <summary>
        /// delete action ouccured will do something
        /// like delete the item in listpath list(which will affect the playlist storing in hardware)
        /// and delete breakpoint(having file in hardware too) of this item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender,EventArgs e) {
            string presentName = null;
            string needDeleteName = null;
            bool flag = true;

            //for loop delete
            int count = listBox1.SelectedItems.Count;
            //delete the read.txt
            File.Delete(str);

            for(int i = 0; i < count; i++) {
                //get the delete name
                string name = Path.GetFileName(listPath[listBox1.SelectedIndex]);


                if(player.URL == listPath[listBox1.SelectedIndex]) {
                    player.Ctlcontrols.stop();
                }

                //must be firstly remove list
                listPath.RemoveAt(listBox1.SelectedIndex);
                //and then remove listbox
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

                //get the updated listpath
                string[] pathstr = listPath.ToArray();
                //and write it again
                File.WriteAllLines(str,pathstr,Encoding.Default);
                string[] processPosition = File.ReadAllLines(process,Encoding.Default);
                if(processPosition.Length > 0 && flag)
                    listProcess = processPosition.ToList();
                foreach(var item in listProcess) {
                    string[] strnew = item.Split(new char[] { '+' },StringSplitOptions.RemoveEmptyEntries);
                    if(strnew[0].Equals(name)) {
                        needDeleteName = item;
                        //listProcess.Remove(item);
                        flag = false;

                    }
                    else if(strnew[0].Equals(currentname)) {
                        presentName = item;
                    }

                }
                listProcess.Remove(needDeleteName);
            }

            listProcess.Remove(presentName);
            if(this.timer1.Enabled == false)
                File.WriteAllLines(process,listProcess.ToArray(),Encoding.Default);
        }



        /// <summary>
        /// player state be changed will affect the timer which is for storing data in hardware
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_PlayStateChange(object sender,AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e) {
            if(player.playState == WMPLib.WMPPlayState.wmppsMediaEnded) {
                //get the listbox seleceted
                int index = listBox1.SelectedIndex;

                //clean the all selected 
                listBox1.SelectedIndices.Clear();
                index++;
                if(index == listBox1.Items.Count) {
                    index = 0;
                }
                //change the index
                listBox1.SelectedIndex = index;
                //assign it to currentname
                currentname = Path.GetFileName(listPath[listBox1.SelectedIndex]);

                //string[] processPosition = File.ReadAllLines(process,Encoding.Default);
                //if(processPosition.Length > 0)
                //    listProcess = processPosition.ToList();

                //asign to URL action must be before to asign the currentposition,otherwise the breakpoint will be not working 
                //because each time the URL setting will reset all the data feild of the player.
                player.URL = listPath[index];
                foreach(var item in listProcess) {
                    string[] strnew = item.Split(new char[] { '+' },StringSplitOptions.RemoveEmptyEntries);
                    if(strnew[0].Equals(currentname)) {
                        //player.Ctlcontrols.currentPosition = Double.Parse(strnew[1]);
                        //currentname = strnew[0];
                        changeTime(Double.Parse(strnew[1]));
                        listProcess.Remove(item);
                        break;
                    }
                }



            }
            if(player.playState == WMPLib.WMPPlayState.wmppsReady) {
                try {
                    player.Ctlcontrols.play();
                }
                catch { }
            }
            if(player.playState == WMPLib.WMPPlayState.wmppsPlaying) {
                timer1.Enabled = true;
            }
            if(player.playState == WMPLib.WMPPlayState.wmppsStopped) {
                timer1.Enabled = false;
            }

        }



        /// <summary>
        /// recursion for getting file and folder as well as their son folder and file
        /// </summary>
        /// <param name="direc"></param>
        private void giveFolderFile(DirectoryInfo direc) {
            FileSystemInfo[] info = direc.GetFileSystemInfos();
            foreach(var item in info) {
                if((Directory.Exists(item.FullName))) {
                    giveFolderFile(new DirectoryInfo(item.FullName));
                }
                else if(File.Exists(item.FullName)) {
                    foreach(var extension in extensionList) {
                        if(item.Extension.Equals(extension)) {
                            if(!listPath.Contains(item.FullName)) {
                                listPath.Add(item.FullName);
                                listBox1.Items.Add(item.Name);
                                File.AppendAllText(str,item.FullName + "\r\n",Encoding.Default);
                                Console.WriteLine(item.Name);
                            }
                        }
                    }

                }
            }
        }


        /// <summary>
        /// import the whole folder including their son folder and file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender,EventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "please select your folder";
            if(dialog.ShowDialog() == DialogResult.OK) {
                string foldPath = dialog.SelectedPath;
                DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                String path = theFolder.FullName;
                giveFolderFile(theFolder);
            }
        }


        /// <summary>
        /// for helping program to store data to hardware 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender,EventArgs e) {
            if(currentname != null && currentname != "") {
                double time = player.Ctlcontrols.currentPosition;
                listProcess.Insert(0,currentname + "+" + time);
                //listProcess.Add(currentname + "+" + time);
                File.WriteAllLines(process,listProcess.ToArray(),Encoding.Default);
                listProcess.Remove(currentname + "+" + time);
            }
        }
        public void changeTime(double time) {
            player.Ctlcontrols.currentPosition = time;
        }
    }


}