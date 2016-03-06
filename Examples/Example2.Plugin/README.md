
This plugin shows how to do more advanced stuff. It:

- Adds a tool window (ToolWindowContent.cs)
- Adds new tree nodes (TreeNodeDataCreator.cs)
- Adds custom tab content for the new AssemblyChildNode tree node (AssemblyChildNodeTabContent.cs). ModuleChildNode implements IDecompileSelf to decompile itself.
- Shows tooltips when hovering over custom references added to the text editor (ToolTipContentCreator.cs)
- Adds a new IDnSpyFile instance and IDnSpyFileNode node (NewDnSpyFile.cs). It opens .txt files and shows the output in the text editor.
