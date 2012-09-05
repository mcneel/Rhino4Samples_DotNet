using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RMA.Rhino;

namespace SampleCsObjectManager
{
  public partial class SampleCsObjectManagerControl : UserControl
  {
    private SampleCsObjectManagerEvents m_events;
    private List<int> m_selected = new List<int>();
    bool m_bInEvent;
    bool m_bInSelect;

    public SampleCsObjectManagerControl()
    {
      InitializeComponent();
      m_bInEvent = false;
      m_bInSelect = false;
      m_events = new SampleCsObjectManagerEvents(this);
    }

    public void SaveSelectedIndices()
    {
      m_selected.Clear();
      ListBox.SelectedIndexCollection selected = m_listbox.SelectedIndices;
      for (int i = 0; i < selected.Count; i++)
        m_selected.Add(selected[i]);

      string str;
      int count = m_selected.Count;
      if (count == 0)
        str = "0 objects selected";
      else if (count == 1)
        str = "1 objects selected";
      else
        str = string.Format("{0} objects selected", count);
      m_count_label.Text = str;
    }

    private void SampleCsObjectManagerControl_Load(object sender, EventArgs e)
    {
      m_events.Register();
      m_events.Enable(true);

      MRhinoObjectIterator it = new MRhinoObjectIterator(
            IRhinoObjectIterator.object_state.undeleted_objects,
            IRhinoObjectIterator.object_category.active_and_reference_objects
            );
      it.IncludeLights(true);
      it.IncludeGrips(false);

      m_listbox.BeginUpdate();
      for (MRhinoObject obj = it.First(); obj != null; obj = it.Next())
      {
        Guid guid = obj.Attributes().m_uuid;
        int index = m_listbox.Items.Add(guid.ToString());
        if (obj.IsSelected() > 0)
          m_listbox.SetSelected(index, true);
      }
      m_listbox.EndUpdate();
      SaveSelectedIndices();
    }

    public void OnCloseDocument(ref MRhinoDoc doc)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      m_listbox.Items.Clear();
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnBeginCommand(IRhinoCommand command, IRhinoCommandContext context)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      // todo
      m_bInEvent = false;
    }

    public void OnEndCommand(IRhinoCommand command, IRhinoCommandContext context, IRhinoCommand.result rc)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      // todo
      m_bInEvent = false;
    }

    public void OnAddObject(ref MRhinoDoc doc, ref MRhinoObject obj)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      Guid guid = obj.Attributes().m_uuid;
      string str = guid.ToString();
      int index = m_listbox.FindStringExact(str);
      if (index == ListBox.NoMatches)
        m_listbox.Items.Add(str);
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnDeleteObject(ref MRhinoDoc doc, ref MRhinoObject obj)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      Guid guid = obj.Attributes().m_uuid;
      string str = guid.ToString();
      int index = m_listbox.FindStringExact(str);
      if (index != ListBox.NoMatches)
        m_listbox.Items.RemoveAt(index);
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnSelectObject(ref MRhinoDoc doc, IRhinoObject obj)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      if (obj != null)
      {
        Guid guid = obj.Attributes().m_uuid;
        int index = m_listbox.FindStringExact(guid.ToString());
        if (index != ListBox.NoMatches)
          m_listbox.SetSelected(index, true);
      }
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnSelectObjects(ref MRhinoDoc doc, IRhinoObject[] objects)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      for (int i = 0; i < objects.Length; i++)
      {
        IRhinoObject obj = objects[i];
        if (obj != null)
        {
          Guid guid = obj.Attributes().m_uuid;
          int index = m_listbox.FindStringExact(guid.ToString());
          if (index != ListBox.NoMatches)
            m_listbox.SetSelected(index, true);
        }
      }
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnDeselectAllObjects(ref MRhinoDoc doc, int count)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      m_listbox.ClearSelected();
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnDeselectObject(ref MRhinoDoc doc, IRhinoObject obj)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      if (obj != null)
      {
        Guid guid = obj.Attributes().m_uuid;
        int index = m_listbox.FindStringExact(guid.ToString());
        if (index != ListBox.NoMatches)
          m_listbox.SetSelected(index, false);
      }
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    public void OnDeselectObjects(ref MRhinoDoc doc, IRhinoObject[] objects)
    {
      if (m_bInSelect)
        return;

      m_bInEvent = true;
      for (int i = 0; i < objects.Length; i++)
      {
        IRhinoObject obj = objects[i];
        if (obj != null)
        {
          Guid guid = objects[i].Attributes().m_uuid;
          int index = m_listbox.FindStringExact(guid.ToString());
          if (index != ListBox.NoMatches)
            m_listbox.SetSelected(index, false);
        }
      }
      SaveSelectedIndices();
      m_bInEvent = false;
    }

    private void BoxEditUserControl_VisibleChanged(object sender, EventArgs e)
    {
      m_events.Enable(this.Visible);
    }

    private void m_listbox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (m_bInEvent)
        return;

      m_bInSelect = true;
      MRhinoDoc doc = RhUtil.RhinoApp().ActiveDoc();

      // Select what got selected
      ListBox.SelectedIndexCollection selected = m_listbox.SelectedIndices;

      // Select what got selected
      int i = 0;
      for (i = 0; i < selected.Count; i++)
      {
        int index = selected[i];
        if (!m_selected.Contains(index))
        {
          Guid guid = new Guid(m_listbox.Items[index].ToString());
          MRhinoObject obj = doc.LookupObject(guid);
          if (obj != null && obj.IsSelectable())
            obj.Select(true);
        }
      }

      // Unselect what got unselected
      for (i = 0; i < m_selected.Count; i++)
      {
        int index = m_selected[i];
        if (!selected.Contains(index))
        {
          Guid guid = new Guid(m_listbox.Items[index].ToString());
          MRhinoObject obj = doc.LookupObject(guid);
          if (obj != null)
            obj.Select(false);
        }
      }

      SaveSelectedIndices();
      doc.Redraw();

      m_bInSelect = false;
    }

    public void SetCountLabel(string message)
    {
      m_count_label.Text = message;
    }
  
  }

}
