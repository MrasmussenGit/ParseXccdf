# ParseXccdf

Command line utility to work with DISA Stig documents and PowerStig.  The goal of this utility is to
- List V-Rule IDs in a single XML file.  This XML file can be the XML from DISA (usually contains the ending xccdf.xml) or the PowerStig post processed XML
- Look for duplicate V-Rules in a single XML document
- Compare pre (xccdf xml from DISA) and post processed documented (processed by PowerStig) and list V Rules missing from one or the other
- Convert a DISA xccdf.xml to a PowerStig like xml.  This is currently a work in progress
