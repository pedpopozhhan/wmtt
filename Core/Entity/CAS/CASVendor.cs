using System;
using System.Globalization;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

public class CASVendor : ICASAudit
{
    public int VENDOR_PK_ID { get; set; }// 	NUMBER(10,0)
    public string VENDOR_ID { get; set; }//	VARCHAR2(10 BYTE)
    public string VENDOR_NAME_SHORT { get; set; }//	VARCHAR2(14 BYTE)
    public string NAME1 { get; set; }//	VARCHAR2(40 BYTE)
    public string NAME2 { get; set; }//	VARCHAR2(40 BYTE)
    public string VENDOR_STATUS { get; set; }//	VARCHAR2(1 BYTE)
    public string VENDOR_PERSISTENCE { get; set; }//	VARCHAR2(1 BYTE)
    public string OLD_VENDOR_ID { get; set; }//	VARCHAR2(15 BYTE)
    public string PRIMARY_VENDOR { get; set; }//	VARCHAR2(10 BYTE)
    public int? PRIM_ADDR_SEQ_NUM { get; set; }//	NUMBER(5,0)
    public string WTHD_TIN_TYPE { get; set; }//	VARCHAR2(1 BYTE)
    public string WTHD_TIN { get; set; }//	VARCHAR2(20 BYTE)
    public string DEFAULT_LOCATION { get; set; }//	VARCHAR2(10 BYTE)
    public string CORPORATE_VENDOR { get; set; }//	VARCHAR2(10 BYTE)
    public string IMG_VENDOR_ID { get; set; }//	VARCHAR2(10 BYTE)
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string UpdateUserId { get; set; }
}