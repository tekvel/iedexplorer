[Setup]
AppName=IED Explorer
AppVersion=1.5
DefaultDirName={pf}\IEDExplorer
DefaultGroupName=IEDExplorer
UninstallDisplayIcon={app}\IEDExplorer.exe
Compression=lzma2
SolidCompression=yes
OutputDir=SetupDir
OutputBaseFilename=IEDExplorer_setup

[Files]
Source: "..\bin\Release_INNO_SETUP\IEDExplorer.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.txt"; DestDir: "{app}"
Source: "..\Iedexplorer.ico"; DestDir: "{app}"
Source: "..\TreeViewAdv\Resources\check.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\DVSplit.cur"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\Folder.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\FolderClosed.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\Leaf.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\loading_icon"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\minus.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\plus.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\uncheck.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\TreeViewAdv\Resources\unknown.bmp"; DestDir: "{app}\TreeViewAdv\Resources"; Flags: ignoreversion
Source: "..\Resources\about.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\arrow_refresh.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\calculator.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\cog.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\computer.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\cross.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\database.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\disk.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\find.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\folder.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\Iedexplorer.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\information.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\magnifier.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\page_copy.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\page_white_database.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\page_white_put.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\page_white_text.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\page_white_text_width.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\run.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\sitemap_color.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\stop.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\tick.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_AddGoose.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Clear.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_ClearSeq.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Export.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_ExportCSV.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_GooseExplorer.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_GooseSender.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Import.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_OpenScl.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Pooling.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Run.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Start.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Stop.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Resources\toolStripButton_Toggle.Image.png"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "..\Docking\DockPanel.bmp"; DestDir: "{app}\Docking\"; Flags: ignoreversion
Source: "..\Docking\Resources\ActiveTabHover_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\ActiveTab_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_Bottom.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\Dockindicator_PaneDiamond_Fill.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\Dockindicator_PaneDiamond_Fill_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_Hotspot.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_HotspotIndex.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_HotspotIndex_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\Dockindicator_PaneDiamond_Hotspot_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_Left.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_Right.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PaneDiamond_Top.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\Dockindicator_PaneDiamond_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelBottom.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelBottom_Active.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelBottom_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelFill.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelFill_Active.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelFill_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelLeft.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelLeft_Active.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelLeft_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelRight.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelRight_Active.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelRight_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelTop.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelTop_Active.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockIndicator_PanelTop_VS2012.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockPane_AutoHide.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockPane_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockPane_Dock.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockPane_Option.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\DockPane_OptionOverflow.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\InactiveTabHover_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\LostFocusTabHover_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\Resources\LostFocusTab_Close.bmp"; DestDir: "{app}\Docking\Resources\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_Bottom.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\Dockindicator_PaneDiamond_Fill.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_Hotspot.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_HotspotIndex.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_Left.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_Right.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PaneDiamond_Top.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelBottom.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelBottom_Active.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelFill.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelFill_Active.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelLeft.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelLeft_Active.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelRight.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelRight_Active.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelTop.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockIndicator_PanelTop_Active.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneCaption_AutoHideNo.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneCaption_AutoHideYes.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneCaption_CloseDisabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneCaption_CloseEnabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_CloseDisabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_CloseEnabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_ScrollLeftDisabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_ScrollLeftEnabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_ScrollRightDisabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Docking\ResourcesVS2003\DockPaneStrip_ScrollRightEnabled.bmp"; DestDir: "{app}\Docking\ResourcesVS2003\"; Flags: ignoreversion
Source: "..\Embed\iec61850.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Embed\iec61850dotnet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Embed\PcapDotNet.Base.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Embed\PcapDotNet.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Embed\PcapDotNet.Core.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Embed\PcapDotNet.Packets.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\IEDExplorer"; Filename: "{app}\IEDExplorer.exe"; WorkingDir: "{userappdata}\IEDExplorer\"; IconFilename: "{app}\Iedexplorer.ico"; IconIndex: 0
Name: "{group}\Uninstall iedexplorer"; Filename: "{uninstallexe}"; WorkingDir: "{app}"

[Dirs]
Name: "{app}\Docking\Helpers\"
Name: "{app}\Docking\Resources\"
Name: "{app}\Docking\ResourcesVS2003\"
Name: "{app}\Docking\Skins\"
Name: "{app}\Docking\Win32\"
