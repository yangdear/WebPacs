using Dicom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dicom.IO.Buffer;
using Dicom.Imaging;

namespace WinViewer
{
    public partial class MainForm : Form
    {
        private DicomFile _file;
        private DicomImage _image;

        public MainForm()
        {
            InitializeComponent();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "DICOM Files (*.dcm;*.dic)|*.dcm;*.dic|All Files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.Cancel) return;

  
            OpenDicomFile(ofd.FileName);
        }


        public void SaveThumbnail(Image src, int newWidth, int newHeight)
        {
            System.Drawing.Image thumbnail = new System.Drawing.Bitmap(newWidth, newHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumbnail);
            g.DrawImage(src, 0, 0, newWidth, newHeight);
            imageList1.Images.Add("", thumbnail);
        }

        private void OpenDicomFile(string fileName)
        {
            DicomFile file = null;

            try
            {
                file = DicomFile.Open(fileName);
            }
            catch (DicomFileException ex)
            {
                file = ex.File;
                MessageBox.Show(
                    this,
                    "Exception while loading DICOM file: " + ex.Message,
                    "Error loading DICOM file",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            if (file != null) OpenFile(file);
        }
        private delegate void AddItemDelegate(string tag, string vr, string length, string value);

        private void AddItem(string tag, string vr, string length, string value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AddItemDelegate(AddItem), tag, vr, length, value);
                return;
            }

            var lvi = lvDicom.Items.Add(tag);
            lvi.SubItems.Add(vr);
            lvi.SubItems.Add(length);
            lvi.SubItems.Add(value);
        }
        private class DumpWalker : IDicomDatasetWalker
        {
            private int _level = 0;

            public DumpWalker(MainForm form)
            {
                Form = form;
                Level = 0;
            }

            public MainForm Form { get; set; }

            public int Level
            {
                get
                {
                    return _level;
                }
                set
                {
                    _level = value;
                    Indent = String.Empty;
                    for (int i = 0; i < _level; i++) Indent += "    ";
                }
            }

            private string Indent { get; set; }

            public void OnBeginWalk()
            {
            }

            public bool OnElement(DicomElement element)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    element.Tag.ToString().ToUpper(),
                    element.Tag.DictionaryEntry.Name);

                string value = "<large value not displayed>";
                if (element.Length <= 2048) value = String.Join("\\", element.Get<string[]>());

                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    var uid = element.Get<DicomUID>(0);
                    var name = uid.Name;
                    if (name != "Unknown") value = String.Format("{0} ({1})", value, name);
                }

                Form.AddItem(tag, element.ValueRepresentation.Code, element.Length.ToString(), value);
                return true;
            }

            public Task<bool> OnElementAsync(DicomElement element)
            {
                return Task.FromResult(this.OnElement(element));
            }

            public bool OnBeginSequence(DicomSequence sequence)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    sequence.Tag.ToString().ToUpper(),
                    sequence.Tag.DictionaryEntry.Name);

                Form.AddItem(tag, "SQ", String.Empty, String.Empty);

                Level++;
                return true;
            }

            public bool OnBeginSequenceItem(DicomDataset dataset)
            {
                var tag = String.Format("{0}Sequence Item:", Indent);

                Form.AddItem(tag, String.Empty, String.Empty, String.Empty);

                Level++;
                return true;
            }

            public bool OnEndSequenceItem()
            {
                Level--;
                return true;
            }

            public bool OnEndSequence()
            {
                Level--;
                return true;
            }

            public bool OnBeginFragment(DicomFragmentSequence fragment)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    fragment.Tag.ToString().ToUpper(),
                    fragment.Tag.DictionaryEntry.Name);

                Form.AddItem(tag, fragment.ValueRepresentation.Code, String.Empty, String.Empty);

                Level++;
                return true;
            }

            public bool OnFragmentItem(IByteBuffer item)
            {
                var tag = String.Format("{0}Fragment", Indent);

                Form.AddItem(tag, String.Empty, item.Size.ToString(), String.Empty);
                return true;
            }

            public Task<bool> OnFragmentItemAsync(IByteBuffer item)
            {
                return Task.FromResult(this.OnFragmentItem(item));
            }

            public bool OnEndFragment()
            {
                Level--;
                return true;
            }

            public void OnEndWalk()
            {
            }

         
        }
        public void OpenFile(DicomFile file)
        {
            try
            {
                lvDicom.BeginUpdate();

                Reset();

                _file = file;

                _image = new DicomImage(file.Dataset);
                for (int i = 0; i < _image.NumberOfFrames; i++)
                {
                    SaveThumbnail(_image.RenderImage(i).As<Image>(), 32, 32);
                    listView1.Items.Add("", i).Tag = _image.RenderImage(i).As<Image>();

                }
                if (listView1.Items.Count > 0)
                    listView1.Items[0].Selected = true;
                new DicomDatasetWalker(_file.FileMetaInfo).Walk(new DumpWalker(this));
                new DicomDatasetWalker(_file.Dataset).Walk(new DumpWalker(this));

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Exception while loading DICOM file: " + ex.Message,
                    "Error loading DICOM file",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                lvDicom.EndUpdate();
            }
        }

        private void Reset()
        {
            lvDicom.Items.Clear();
            imageList1.Images.Clear();
            listView1.Items.Clear();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count>0)
            pictureBox1.Image = (Image)listView1.SelectedItems[0].Tag;
        }
    }
}
