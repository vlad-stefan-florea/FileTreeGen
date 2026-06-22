function toggleTheme() {
      document.body.classList.toggle("dark-mode");
}
function expandAll() {
  const folders = Array.from(nodeMap.values())
    .filter((node) => node.Type === 0)
    .sort((a, b) => a.Id - b.Id);
  folders.forEach((folder) => {
    const folderBtn = document.getElementById(`node-${folder.Id}`);
    if (folderBtn && folderBtn.getAttribute("data-state") === "closed") {
      handleFolderClick(folder.Id);
    }
  });
}
function collapseAll() {
  const root = document.getElementById("node-0");
  root.remove();
  initTree();
}