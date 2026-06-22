const nodeMap = new Map(nodeList.map((node) => [node.Id, node]));
function initTree() {
  const rootNode = nodeMap.get(0);
  document.getElementById("treeContainer").innerHTML =
    createHeader(rootNode);
}
function createHeader(node) {
  if (node.Type === 0) {
    return `
    <button class="h" id="node-${node.Id}" data-state="closed" onclick="handleFolderClick(${node.Id})">
      <span class="ico">
        ${ICONS["folder_closed"]}
      </span>
      ${node.Name}
      <span class="ind">[+]</span>
    </button>
  `;
  } else {
    const ext = getFileExt(node.Path); 
    const iconKey = EXTENSION_MAP[ext] || "default";
    return `
    <a href="file:///${node.Path}" class="h" id="node-${node.Id}" target="_blank">
      <span class="ico">
        ${ICONS[iconKey] || ICONS["default"]}
      </span>
      ${node.Name}
    </a>
  `;
  }
}
function handleFolderClick(folderId) {
  const folderBtn = document.getElementById(`node-${folderId}`);
  const state = folderBtn.getAttribute("data-state");
  const iconWrapper = folderBtn.querySelector(".folder-icon-wrapper");
  const indicator = folderBtn.querySelector(".ind");
  if (state === "closed") {
    const children = Array.from(nodeMap.values()).filter(
      (node) => node.ParentId === folderId,
    );
    const childrenHTML = children
      .map((child) => createHeader(child))
      .join("");
    const contents = `
      <div class="c" id="contents-${folderId}">
        ${childrenHTML}
      </div>
    `;
    folderBtn.insertAdjacentHTML("afterend", contents);
    folderBtn.setAttribute("data-state", "open");
    if (iconWrapper) iconWrapper.innerHTML = ICONS["folder_open"];
    if (indicator) indicator.textContent = "[-]";
  } else {
    removeChildrenFromDOM(folderId);
    folderBtn.setAttribute("data-state", "closed");
    if (iconWrapper) iconWrapper.innerHTML = ICONS["folder_closed"];
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