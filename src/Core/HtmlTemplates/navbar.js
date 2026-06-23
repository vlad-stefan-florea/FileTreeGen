const nodesLimit = 5000;
function toggleTheme() {
  const treeState = document
    .getElementById("treeContainer")
    .getAttribute("data-state");
  if (nodeList.length > nodesLimit && treeState === "expanded-full") {
    const proceed = confirm(
      `Warning: This report is fully expanded (${nodeList.length} items). Changing the theme will heavily impact performance. Proceed?`,
    );
    if (!proceed) return;
  }
  document.body.classList.toggle("dark-mode");
}
function expandAll() {
  if (nodeList.length > nodesLimit) {
    const proceed = confirm(
      `Warning: This report contains ${nodeList.length} items. Expanding everything may freeze the browser and heavily impact performance. Proceed?`,
    );
    if (!proceed) return;
  }
  const closedFolders = Array.from(nodeMap.values())
    .filter((node) => node.Type === 0 && parentsWithChildren.has(node.Id))
    .sort((a, b) => a.Id - b.Id);
  if (closedFolders.length === 0) return;
  let currentIndex = 0;
  const chunkSize = 10;

  document.body.style.cursor = "wait";
  function renderChunk() {
    const limit = Math.min(currentIndex + chunkSize, closedFolders.length);
    for (; currentIndex < limit; currentIndex++) {
      const folder = closedFolders[currentIndex];
      const folderBtn = document.getElementById(`node-${folder.Id}`);
      if (folderBtn && folderBtn.getAttribute("data-state") === "closed") {
        handleFolderClick(folder.Id);
      }
    }
    if (currentIndex < closedFolders.length) {
      requestAnimationFrame(renderChunk);
    } else {
      document.body.style.cursor = "initial";
      document
        .getElementById("treeContainer")
        .setAttribute("data-state", "expanded-full");
    }
  }
  if (closedFolders.length > 0) {
    requestAnimationFrame(renderChunk);
  }
}
function collapseAll() {
  const root = document.getElementById("node-0");
  root.remove();
  document
    .getElementById("treeContainer")
    .setAttribute("data-state", "collapsed-full");
  initTree();
}
function renderNavbar() {
  const html = `
      <div class="navbar">
        <h3>'___dirName___' folder structure report</h3>
        <button onclick="expandAll()" aria-label="Expand all nodes">
          <span class="ico">${ICONS["expand"] || ""}</span>
          Expand All
        </button>
        <button onclick="collapseAll()" aria-label="Collapse all nodes">
           <span class="ico">${ICONS["collapse"] || ""}</span>
           Collapse All
        </button>
        <button onclick="toggleTheme()" aria-label="Change Theme">
          <span class="ico">${ICONS["theme"] || ""}</span>
          Theme
        </button>
      </div>
      `;
  document.body.insertAdjacentHTML("afterbegin", html);
}
renderNavbar();
