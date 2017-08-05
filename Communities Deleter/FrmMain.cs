using System;
using System.Linq;
using System.Windows.Forms;
using PSNLib;
using static System.Threading.Tasks.Task;
namespace Communities_Deleter
{
    public partial class FrmMain : Form
    {
        private PSAPI _psapi;
        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            btnLogin.Click += BtnLogin_Click;
            btnRefresh.Click += delegate { Execute(RetrieveData); };
            #region ContextMenuStrip cms
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Delete");
            cms.Click += delegate
            {
                Execute(() =>
                {
                    try
                    {
                        foreach (ListViewItem community in listView.SelectedItems)
                        {
                            var threadId = community.SubItems[3].Text;
                            if (_psapi.Communites.Leave(threadId))
                            {
                                community.Remove();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            };
            #endregion
            #region listView
            listView.View = View.Details;
            listView.GridLines = true;
            listView.FullRowSelect = true;
            listView.ShowItemToolTips = true;
            int lstViewWidth = listView.Size.Width;
            listView.Columns.Add("*", 50);
            lstViewWidth -= 50;
            lstViewWidth /= 2;
            listView.Columns.Add("Community", lstViewWidth);
            listView.Columns.Add("MembersCount", lstViewWidth);
            listView.ContextMenuStrip = cms;
            listView.KeyDown += (sender, e) =>
            {
                if (e.KeyCode != Keys.A || !e.Control) return;
                listView.MultiSelect = true;
                foreach (ListViewItem item in listView.Items)
                {
                    item.Selected = true;
                }
            };
            #endregion
        }
        private void PicBox(bool state)
        {
            pictureBox.Visible = state;
        }
        private bool LogIn()
        {
            try
            {
                 _psapi = new PSAPI(txtEmailBox.Text, txtPassBox.Text);
                lblUsername.Text = _psapi.Profile.OnlineId;
                return  _psapi != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void LockMe()
        {
            bool stat = btnLogin.Enabled;
            btnLogin.Enabled = !stat;
            PicBox(stat);
            Controls.OfType<Control>().ToList().ForEach(x =>
            {
                if (x.GetType() != typeof(PictureBox))
                    x.Enabled = !stat;
            });
        }
        private async void Execute(Action act)
        {
            LockMe();
            await Run(act);
            LockMe();
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtEmailBox.TextLength < 1 || txtPassBox.TextLength < 1)
            {
                MessageBox.Show("Error, Fill the details..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Execute(() =>
            {
                if (LogIn())
                {
                    RetrieveData();
                }
            });
        }

        private void RetrieveData()
        {
            if (_psapi == null)
                return;
            listView.Items.Clear();
            var allCommunities = _psapi.Communites.GetMyCommunities();
            int communitiesLength = allCommunities.Length;
            lblCommunitiesCount.Text = communitiesLength.ToString();
            for (int i = 0; i < communitiesLength; i++)
            {
                listView.Items.Add(new ListViewItem(new[]
                    {(i + 1).ToString(), $"{allCommunities[i].Name} - [{(allCommunities[i].IsOpen ? "Open" : "Close")}]", allCommunities[i].MembersCount.ToString() , allCommunities[i].Id}));
            }
        }
    }
}
