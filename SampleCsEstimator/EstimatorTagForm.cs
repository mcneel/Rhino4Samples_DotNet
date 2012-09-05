using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Estimator
{
  public partial class EstimatorTagForm : Form
  {
    private ListViewColumnSorter m_sorter;
    public string m_str_title = "Apply Linear Tags";
    public string m_str_prompt = "Select one or more linear tags.";
    public EstimatorTag.tag_type m_type = EstimatorTag.tag_type.linear_tag;
    public List<int> m_selected_tags;

    public EstimatorTagForm()
    {
      InitializeComponent();
      m_sorter = new ListViewColumnSorter();
      m_list.ListViewItemSorter = m_sorter;
      m_selected_tags = new List<int>(8);
    }

    private void EstimatorTagForm_Load(object sender, EventArgs e)
    {
      if (this.m_type == EstimatorTag.tag_type.item_tag)
        this.m_list.MultiSelect = false;

      this.Text = m_str_title;
      m_prompt.Text = m_str_prompt;

      EstimatorPlugIn plugin = RMA.Rhino.RhUtil.GetPlugInInstance() as EstimatorPlugIn;
      for (int i = 0; i < plugin.m_tag_table.TagCount(); i++)
      {
        EstimatorTag tag = plugin.m_tag_table[i];
        if (tag.Type() == m_type)
        {
          ListViewItem item = new ListViewItem(tag.Id());
          item.SubItems.Add(tag.Description());
          item.Tag = i;
          m_list.Items.Add(item);
        }
      }

      m_list.Columns.Add("Tag");
      m_list.Columns.Add("Description");

      foreach (ColumnHeader ch in m_list.Columns)
      {
        ch.Width = -2;
      }
    }

    private void m_list_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (e.Column == m_sorter.SortColumn)
      {
        if (m_sorter.Order == SortOrder.Ascending)
          m_sorter.Order = SortOrder.Descending;
        else
          m_sorter.Order = SortOrder.Ascending;
      }
      else
      {
        m_sorter.SortColumn = e.Column;
        m_sorter.Order = SortOrder.Ascending;
      }
      this.m_list.Sort();
    }

    private void m_ok_Click(object sender, EventArgs e)
    {
      foreach (int index in m_list.SelectedIndices)
      {
        ListViewItem item = m_list.Items[index];
        m_selected_tags.Add(Convert.ToInt32(item.Tag));
      }
    }
  }
}