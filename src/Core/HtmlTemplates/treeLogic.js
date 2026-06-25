const nodeMap = new Map(nodeList.map((node) => [node.Id, node]));
function initTree() {
  const rootNode = nodeMap.get(0);
  document.getElementById("treeContainer").innerHTML = createHeader(rootNode);
}
function createIdleHeader(node, message) {
  return `
        <button class="h" id="node-${node.Id}" data-state="empty" style='cursor:default' aria-label='Empty Folder: ${node.Name}'>
          ${noIcons ? "" : `<span class="ico">${ICONS["folder_closed"]}</span>`}
          ${node.Name}
          <span class='ind' style='opacity:0.5'>(${message})</span>
        </button>
    `;
}
function createHeader(node) {
  if (!node.IsFile) {
    if (node.IsEmptyDir) {
      return createIdleHeader(node, "Empty");
    } else if (node.IsSkipped) {
      return createIdleHeader(node, "Skipped");
    } else if (node.IsUnscanned) {
      return createIdleHeader(node, "Not Scanned");
    } else
      return `
              <button class="h" id="node-${node.Id}" data-state="closed" onclick="handleFolderClick(${node.Id})" aria-label='Expand folder: ${node.Name}'>
                ${noIcons ? "" : `<span class="ico">${ICONS["folder_closed"]}</span>`}
                ${node.Name}
                <span class='ind'>[+]</span>
              </button>
            `;
  } else {
    const ext = getFileExt(node.Path);
    const iconKey = EXTENSION_MAP[ext] || "default";
    return `
            <a href="file:///${node.Path}" class="h" id="node-${node.Id}" target="_blank" aria-label='Open File: ${node.Name}'>
              ${noIcons ? "" : `<span class="ico">${ICONS[iconKey] || ICONS["default"]}</span>`}
              ${node.Name}
            </a>
          `;
  }
}
function handleFolderClick(folderId) {
  const folderBtn = document.getElementById(`node-${folderId}`);
  const state = folderBtn.getAttribute("data-state");
  const iconWrapper = folderBtn.querySelector(".ico");
  const indicator = folderBtn.querySelector(".ind");
  if (state === "closed") {
    const children = Array.from(nodeMap.values()).filter(
      (node) => node.ParentId === folderId,
    );
    const childrenHTML = children.map((child) => createHeader(child)).join("");
    const contents = `
      <div class="c" id="contents-${folderId}">
        ${childrenHTML}
      </div>
    `;
    folderBtn.insertAdjacentHTML("afterend", contents);
    folderBtn.setAttribute("data-state", "open");
    folderBtn.setAttribute(
      "aria-label",
      folderBtn.getAttribute("aria-label").replace("Expand", "Collapse"),
    );
    if (iconWrapper && !noIcons) iconWrapper.innerHTML = ICONS["folder_open"];
    if (indicator) indicator.textContent = "[-]";
  } else {
    removeChildrenFromDOM(folderId);
    folderBtn.setAttribute("data-state", "closed");
    folderBtn.setAttribute(
      "aria-label",
      folderBtn.getAttribute("aria-label").replace("Collapse", "Expand"),
    );
    if (iconWrapper && !noIcons) iconWrapper.innerHTML = ICONS["folder_closed"];
    if (indicator) indicator.textContent = "[+]";
  }
}
function removeChildrenFromDOM(parentFolderId) {
  const contentsContainer = document.getElementById(
    `contents-${parentFolderId}`,
  );
  if (contentsContainer) {
    contentsContainer.remove();
  }
}
function getFileExt(filePath) {
  return filePath.split(".").pop().toLowerCase();
}
initTree();
