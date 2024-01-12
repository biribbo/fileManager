using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mime;
using System.Web;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Collections;
using System.Reflection;
using System.Data.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms.VisualStyles;

namespace menagerPlików
{
	public partial class mainForm : Form
	{
		string lv1_currentDirectory;
		string lv2_currentDirectory;
		Boolean isLv1SortedAscending;
		Boolean isLv2SortedAscending;
		Boolean enterPressed = false;

		public System.Windows.Forms.ListView lv1 { get { return listView1; } set { listView1 = value; } }
		public System.Windows.Forms.ListView lv2 { get { return listView2; } set { listView2 = value; } }

		public mainForm()
		{
			InitializeComponent();
			DirectoryInfo rootDir = new DirectoryInfo(@"C:\");
			wypelnijliste(listView1, rootDir.FullName);
			wypelnijliste(listView2, rootDir.FullName);
			isLv1SortedAscending = true;
			isLv2SortedAscending = true;
		}

		public void wypelnijliste(System.Windows.Forms.ListView lv, string path)
		{
			lv.Items.Clear();

			DirectoryInfo dir = new DirectoryInfo(path);

			try
			{
				if (dir.Parent != null)
				{
					ListViewItem parent = new ListViewItem();
					parent.ImageKey = "prev";
					parent.SubItems.Add("...");
					lv.Items.Add(parent);
				}

				foreach (var directory in dir.GetDirectories())
				{
					ListViewItem directoryItem = new ListViewItem();
					directoryItem.ImageKey = "dir";
					directoryItem.SubItems.Add(directory.Name);
					directoryItem.SubItems.Add(directory.CreationTime.ToString());
					lv.Items.Add(directoryItem);
				}
				foreach (var file in dir.GetFiles())
				{
					ListViewItem fileItem = new ListViewItem();
					StringCollection imageExtensions = new StringCollection();
					String[] imgArr = new String[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".svg", ".webp", ".avif", ".apng" };
					imageExtensions.AddRange(imgArr);
					if (imageExtensions.Contains(file.Extension))
						fileItem.ImageKey = "image";
					else
						fileItem.ImageKey = "file";
					fileItem.SubItems.Add(file.Name);
					fileItem.SubItems.Add(file.CreationTime.ToString());
					lv.Items.Add(fileItem);
				}
				if (lv == listView1)
					lv1_currentDirectory = dir.FullName;
				else
					lv2_currentDirectory = dir.FullName;
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show(ex.Message);
				wypelnijliste(lv, dir.Parent.FullName);
			}
			textBox1.Text = lv1_currentDirectory;
			textBox2.Text = lv2_currentDirectory;
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (listView1.SelectedItems[0] == listView1.Items[0])
			{
				DirectoryInfo dir = new DirectoryInfo(lv1_currentDirectory);
				wypelnijliste(listView1, dir.Parent.FullName + @"\");
				return;
			}

			string name = lv1_currentDirectory;
			name += listView1.SelectedItems[0].SubItems[1].Text;
			if (Directory.Exists(name))
			{
				name += @"\";
				DirectoryInfo dir = new DirectoryInfo(name);
				listView1.ListViewItemSorter = null;
				wypelnijliste(listView1, dir.FullName);

			}
			else if (File.Exists(name))
			{
				System.Diagnostics.Process.Start(name);
			}
			else
			{
				MessageBox.Show(name + " nie istnieje");
			}
		}

		private void listView2_DoubleClick(object sender, EventArgs e)
		{
			if (listView2.SelectedItems[0] == listView2.Items[0])
			{
				DirectoryInfo dir = new DirectoryInfo(lv2_currentDirectory);
				wypelnijliste(listView2, dir.Parent.FullName + @"\");
				return;
			}

			string name = lv2_currentDirectory;
			name += listView2.SelectedItems[0].SubItems[1].Text.ToString();
			if (Directory.Exists(name))
			{
				name += @"\";
				DirectoryInfo dir = new DirectoryInfo(name);
				listView2.ListViewItemSorter = null;
				wypelnijliste(listView2, dir.FullName);
			}
			else if (File.Exists(name))
			{
				System.Diagnostics.Process.Start(name);
			}
			else
			{
				MessageBox.Show(name + " nie istnieje");
			}
		}

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.listView1.ListViewItemSorter = new ListViewItemSorter(e.Column, isLv1SortedAscending);
			if (isLv1SortedAscending)
				isLv1SortedAscending = false;
			else
				isLv1SortedAscending = true;
		}

		private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.listView2.ListViewItemSorter = new ListViewItemSorter(e.Column, isLv2SortedAscending);
			if (isLv2SortedAscending)
				isLv2SortedAscending = false;
			else
				isLv2SortedAscending = true;
		}

		private void mainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode.ToString() == "F7")
			{
				addForm add = null;
				if (listView2.SelectedItems.Count > 0)
					add = new addForm(this, listView2, lv2_currentDirectory);
				else
					add = new addForm(this, listView1, lv1_currentDirectory);
				DialogResult dr = add.ShowDialog();
				if (dr == DialogResult.OK)
				{
					wypelnijliste(listView1, lv1_currentDirectory);
					wypelnijliste(listView2, lv2_currentDirectory);
				}
			}
			if (e.KeyCode.ToString() == "F8")
			{
				int lv1_count = listView1.SelectedItems.Count;
				int lv2_count = listView2.SelectedItems.Count;
				if (lv1_count > 0)
					DeleteItem(listView1, lv1_count, lv1_currentDirectory);
				else if (lv2_count > 0)
					DeleteItem(listView2, lv2_count, lv2_currentDirectory);
				wypelnijliste(listView1, lv1_currentDirectory);
				wypelnijliste(listView2, lv2_currentDirectory);
			}
		}
		public void DeleteItem(System.Windows.Forms.ListView lv, int count, string cd)
		{
			string name = "";
			try
			{
				if (count == 1)
				{
					name = cd + lv.SelectedItems[0].SubItems[1].Text;
					DialogResult dialogResult = MessageBox.Show("Czy na pewno chcesz usunąć " + name + "?", "Usuwanie", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes)
					{
						if (File.Exists(name))
							File.Delete(name);
						else if (Directory.Exists(name))
							Directory.Delete(name, true);
					}
				}
				else if (count > 1)
				{
					DialogResult dialogResult = MessageBox.Show("Czy na pewno chcesz usunąć " + count + " wskazanych elementów?", "Usuwanie", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes)
					{
						foreach (ListViewItem x in lv.SelectedItems)
						{
							name = cd + x.SubItems[1].Text;
							if (File.Exists(name))
								File.Delete(name);
							else if (Directory.Exists(name))
								Directory.Delete(name, true);
							else
								MessageBox.Show(name + " nie istnieje.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			addForm add = null;
			if (listView2.SelectedItems.Count > 0)
				add = new addForm(this, listView2, lv2_currentDirectory);
			else
				add = new addForm(this, listView1, lv1_currentDirectory);
			DialogResult dr = add.ShowDialog();
			if (dr == DialogResult.OK)
			{
				wypelnijliste(listView1, lv1_currentDirectory);
				wypelnijliste(listView2, lv2_currentDirectory);
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			int lv1_count = listView1.SelectedItems.Count;
			int lv2_count = listView2.SelectedItems.Count;
			if (lv1_count > 0)
				DeleteItem(listView1, lv1_count, lv1_currentDirectory);
			else if (lv2_count > 0)
				DeleteItem(listView2, lv2_count, lv2_currentDirectory);
			wypelnijliste(listView1, lv1_currentDirectory);
			wypelnijliste(listView2, lv2_currentDirectory);
		}

		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				listView1_DoubleClick(sender, e);
			else if (e.KeyCode == Keys.Back)
			{
				DirectoryInfo dir = new DirectoryInfo(lv1_currentDirectory);
				wypelnijliste(listView1, dir.Parent.FullName + @"\");
			}
		}

		private void listView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				listView2_DoubleClick(sender, e);
			else if (e.KeyCode == Keys.Back)
			{
				DirectoryInfo dir = new DirectoryInfo(lv2_currentDirectory);
				wypelnijliste(listView2, dir.Parent.FullName + @"\");
			}
		}

		private void ListView_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(System.Windows.Forms.ListView.SelectedListViewItemCollection)))
			{
				e.Effect = DragDropEffects.Copy;
			}
        }

		private void listView1_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

				try
				{
					foreach (string path in paths)
					{
						if (Directory.Exists(path))
						{
							string dirPath = path + @"\";
							DirectoryInfo source = new DirectoryInfo(dirPath);
							DirectoryInfo target = new DirectoryInfo(lv1_currentDirectory + source.Name + @"\");
							CopyDir(source, target);
						}
						else if (File.Exists(path))
						{
							FileInfo file = new FileInfo(path);
							File.Copy(path, lv1_currentDirectory + file.Name, true);
							textBox2.Text = path;
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

				wypelnijliste(listView1, lv1_currentDirectory);
			}
		}
		private void listView2_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

				try
				{
					foreach (string path in paths)
					{
						if (Directory.Exists(path))
						{
							string dirPath = path + @"\";
							DirectoryInfo source = new DirectoryInfo(dirPath);
							DirectoryInfo target = new DirectoryInfo(lv2_currentDirectory + source.Name + @"\");
							CopyDir(source, target);
						}
						else if (File.Exists(path))
						{
							FileInfo file = new FileInfo(path);
							File.Copy(path, lv2_currentDirectory + file.Name, true);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

				wypelnijliste(listView2, lv2_currentDirectory);
			}
		}

		private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			System.Windows.Forms.ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
			if (items.Count > 0)
			{
				List<string> files = new List<string>();
				foreach (ListViewItem item in items)
				{
					string file = lv1_currentDirectory + item.SubItems[1].Text;
					files.Add(file);
				}

				DataObject data = new DataObject();
				data.SetData(DataFormats.FileDrop, files.ToArray());
				listView1.DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void listView2_ItemDrag(object sender, ItemDragEventArgs e)
		{
			System.Windows.Forms.ListView.SelectedListViewItemCollection items = listView2.SelectedItems;
			if (items.Count > 0)
			{
				List<string> files = new List<string>();
				foreach (ListViewItem item in items)
				{
					string file = lv2_currentDirectory + item.SubItems[1].Text;
					files.Add(file);
				}

				DataObject data = new DataObject();
				data.SetData(DataFormats.FileDrop, files.ToArray());
				listView2.DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		public void CopyDir(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);
			foreach (FileInfo file in source.GetFiles())
			{
				File.Copy(file.FullName, target.FullName + file.Name, true);
			}
			foreach (DirectoryInfo subDir in source.GetDirectories())
			{
				DirectoryInfo targetSubDir = target.CreateSubdirectory(subDir.Name);
				CopyDir(subDir, targetSubDir);
			}
		}
    }

    public class ListViewItemSorter : IComparer
	{
		private int col;
		public int Col { get { return col; } set { col = value; } }

		private Boolean isAsc;
		public Boolean IsAsc { get { return isAsc; } set { isAsc = value; } }

		public ListViewItemSorter()
		{
			col = 0;
			isAsc = true;
		}
		public ListViewItemSorter(int c, bool ia)
		{
			col = c;
			isAsc = ia;
		}

		public int Compare(object x, object y)
		{
			ListViewItem lviX = x as ListViewItem;
			ListViewItem lviY = y as ListViewItem;

            if (lviX.SubItems[1].Text == "...")
				return -1;
            if (lviY.SubItems[1].Text == "...")
				return 1;

			int result;

			if (lviX == null && lviY == null)
				result = 0;
			else if (lviX == null)
                result = -1;
			else if (lviY == null || lviY.SubItems[1].Text == "...")
				result = 1;
			else
			{
				DateTime date1, date2;
				if (DateTime.TryParse(lviX.SubItems[col].Text, out date1) && DateTime.TryParse(lviY.SubItems[col].Text, out date2))
					result = DateTime.Compare(date1, date2);
				else
					result = String.Compare(lviX.SubItems[col].Text, lviY.SubItems[col].Text);
			}

            if (isAsc)
				return result;
			else
				return -result;
		}
	}
}