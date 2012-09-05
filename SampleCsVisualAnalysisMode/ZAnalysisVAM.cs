using System;
using System.Collections.Generic;
using System.Text;
using RMA.OpenNURBS;
using RMA.Rhino;

namespace ZAnalysis
{
  /// <summary>
  /// Like command classes, Rhino will create your
  /// MRhinoVisualAnalysisMode-inherited classes
  /// for you automatically. So it is critical that this
  /// class be declared as public.
  /// </summary>
  public class ZAnalysisVAM : MRhinoVisualAnalysisMode
  {
    // This simple example provides a false color based on the
    // world z-coordinate. For details, see the implementation
    // of the FalseColor() function.

    /// <summary>
    /// The unique identifier of our analysis mode
    /// </summary>
    public static Guid ZANALYSIS_VAM_ID = new Guid("{27012730-ED34-4CDD-9F3A-57E0FE693FC9}");
    
    /// <summary>
    /// Private variables
    /// </summary>
    OnInterval m_z_range;
    OnInterval m_hue_range;
    bool m_bShowIsoCurves;

    /// <summary>
    /// Public constructor
    /// </summary>
    public ZAnalysisVAM()
      : base(ZANALYSIS_VAM_ID, IRhinoVisualAnalysisMode.analysis_style.false_color_style)
    {
      m_z_range = new OnInterval(-10.0, 10.0);
      m_hue_range = new OnInterval(0.0, 4.0 * OnUtil.On_PI / 3.0); // red to green to blue
      m_bShowIsoCurves = true;
    }

    /// <summary>
    /// MRhinoVisualAnalysisMode override
    /// </summary>
    public override string GetAnalysisModeName()
    {
      return "Z-Analysis";
    }

    /// <summary>
    /// MRhinoVisualAnalysisMode override
    /// </summary>
    public override bool ObjectSupportsAnalysisMode(IRhinoObject obj)
    {
      bool rc = false;
      if (null != obj)
      {
        switch (obj.ObjectType())
        {
          case IOn.object_type.mesh_object:
            rc = true;
            break;

          case IOn.object_type.surface_object:
          case IOn.object_type.polysrf_filter:
          case IOn.object_type.brep_object:
            {
              if (null != MRhinoBrepObject.ConstCast(obj))
                rc = true;
            }
            break;
        }
      }
      return rc;
    }

    /// <summary>
    /// MRhinoVisualAnalysisMode override
    /// </summary>
    public override void UpdateVertexColors(IRhinoObject obj, OnMesh[] meshes)
    {
      // Rhino calls this function when it is time for you
      // to set the false colors on the analysis mesh vertices.
      // For breps, there is one mesh per face.  For mesh objects,
      // there is a single mesh.
      int count = meshes.Length;
      if (count > 0 )
      {
        // A "mapping tag" is used to determine if the colors need to be set
        OnMappingTag mt = MappingTag();
        for ( int mi = 0; mi < count; mi++ )
        {
          OnMesh mesh = meshes[mi];
          if (null != mesh && 0 != mt.Compare(mesh.m_Ctag) )
          {
            // The mesh's mapping tag is different from ours. Either
            // the mesh has no false colors, has false colors set by
            // another analysis mode, has false colors set using
            // different m_z_range[]/m_hue_range[] values, or the
            // mesh has been moved.  In any case, we need to set
            // the false colors to the ones we want.
            int vcount = mesh.m_V.Count();
            ArrayOnColor vertex_colors = mesh.m_C;
            vertex_colors.SetCount(0);     // in case something else had set the colors
            vertex_colors.Reserve(vcount); // for efficiency
            for (int vi = 0; vi < vcount; vi++ )
            {
              double z = mesh.m_V[vi].z;
              OnColor color = FalseColor(z);
              vertex_colors.Append(color);
            }

            // set the mesh's color tag 
            mesh.m_Ctag = mt;
          }
        }
      }
    }

    /// <summary>
    /// MRhinoVisualAnalysisMode override
    /// </summary>
    public override bool ShowIsoCurves()
    {
      // Most shaded analysis modes that work on breps have
      // the option of showing or hiding isocurves.  Run the
      // built-in Rhino ZebraAnalysis to see how Rhino handles
      // the user interface.  If controlling iso-curve visability 
      // is a feature you want to support, then provide user
      // interface to set this member variable.
      return m_bShowIsoCurves;
    }

    /// <summary>
    /// Returns a mapping tag that is used to detect when
    /// a meshes colors need to be set.  For details, see the
    /// implementation  of MappingTag and UpdateVertexColors.
    /// </summary>
    /// <returns></returns>
    public OnMappingTag MappingTag()
    {
      OnMappingTag mt = new OnMappingTag();

      // Since the false colors that are shown will change if
      // the mesh is transformed, we have to initialize the
      // transformation.
      mt.m_mesh_xform.Identity();

      // This is the analysis mode id passed to the 
      // CRhinoVisualAnalysisMode constructor. Use the
      // m_am_id member and it this code will alwasy 
      // work correctly.
      mt.m_mapping_id = m_am_id;

      // This is a 32 bit CRC or the information used to
      // set the false colors.
      // For this example, the m_z_range and m_hue_range
      // intervals control the colors, so we calculate 
      // their crc.
      mt.m_mapping_crc = 0;
      mt.m_mapping_crc = Crc32.ComputeChecksum(m_z_range);
      mt.m_mapping_crc = Crc32.ComputeChecksum(m_hue_range);
      return mt;
    }

    OnColor FalseColor(double z)
    {
      // Simple example of one way to change a number into a color.
      double s = m_z_range.NormalizedParameterAt(z);
      if (s < 0.0)
        s = 0.0; 
      else if (s > 1.0)
        s = 1.0;
      double hue = m_hue_range.ParameterAt(s);
      OnColor color = new OnColor();
      color.SetHSV(hue, 1.0, 1.0);
      return color;
    }
  }

  /// <summary>
  /// The openNURBS toolkit (C++) has a ON_CRC32 function that could normally
  /// be used to compute a CRC checksum. But the function was not exported
  /// to the .NET SDK. But, this class should do nicely.
  /// </summary>
  class Crc32
  {
    readonly static uint[] m_table = new uint[]
    {
      0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
      0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
      0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
      0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
      0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
      0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
      0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
      0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
      0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
      0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
      0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
      0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
      0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
      0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
      0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
      0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
      0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
      0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
      0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
      0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
      0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
      0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
      0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
      0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
      0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
      0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
      0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
      0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
      0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
      0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
      0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
      0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
      0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
      0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
      0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
      0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
      0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
      0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
      0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
      0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
      0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
      0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
      0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
      0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
      0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
      0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
      0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
      0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
      0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
      0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
      0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
      0x2D02EF8D
    };

    public static uint ComputeChecksum(OnInterval dom)
    {
      if (null != dom)
        return ComputeChecksum(BitConverter.GetBytes(dom.Max() - dom.Min()));
      return 0;
    }

    public static uint ComputeChecksum(byte[] bytes)
    {
      uint crc = 0xffffffff;
      for (int i = 0; i < bytes.Length; ++i)
      {
        byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
        crc = (uint)((crc >> 8) ^ m_table[index]);
      }
      return ~crc;
    }
  }

}
