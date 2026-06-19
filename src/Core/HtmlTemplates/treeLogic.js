function toggle(element) {
      const content = element.nextElementSibling;
      if (!content) return;
      const icon = element.querySelector(".toggle-icon");
      const folderName = element.getAttribute("data-name");

      if (content.classList.contains("hidden")) {
        content.classList.remove("hidden");
        icon.innerText = "[-] 📂";
        element.setAttribute("aria-label", `Collapse Directory: ${folderName}`);
        element.setAttribute("aria-expanded", "true");
      } else {
        content.classList.add("hidden");
        icon.innerText = "[+] 📁";
        element.setAttribute("aria-label", `Expand Directory: ${folderName}`);
        element.setAttribute("aria-expanded", "false");
      }
    }

    function toggleTheme() {
      document.body.classList.toggle("dark-mode");
    }

    function expandAll() {
      document
        .querySelectorAll(".folder-content")
        .forEach((el) => el.classList.remove("hidden"));
      document
        .querySelectorAll(".toggle-icon.hasContents")
        .forEach((el) => (el.innerText = "[-] 📂"));
    }

    function collapseAll() {
      document
        .querySelectorAll(".folder-content")
        .forEach((el) => el.classList.add("hidden"));
      document
        .querySelectorAll(".toggle-icon.hasContents")
        .forEach((el) => (el.innerText = "[+] 📁"));
    }