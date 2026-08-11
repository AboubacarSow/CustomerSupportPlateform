/**
 * Systek Chatbot Widget
 * Embeddable, dependency-free chat widget.
 *
 * Usage:
 *   <script
 *     src="https://your-cdn.example.com/chatbot-widget.js"
 *     data-api-url="https://api.systek.example"
 *     data-primary-color="#2b6cb0"
 *     data-title="Systek Support"
 *     data-greeting="Hi! How can I help you today?"
 *     defer
 *   ></script>
 *
 * All data-* attributes are optional except data-api-url.
 */
(function () {
  "use strict";

  var CONVERSATION_STORAGE_KEY = "systek_chat_conversation_id";

  function getCurrentScript() {
    // document.currentScript is null by the time async callbacks run,
    // so capture it immediately at parse time.
    return document.currentScript;
  }

  var scriptEl = getCurrentScript();

  function readConfig(scriptEl) {
    var ds = (scriptEl && scriptEl.dataset) || {};
    return {
      apiUrl: (ds.apiUrl || "").replace(/\/$/, ""),
      primaryColor: ds.primaryColor || "#2b6cb0",
      title: ds.title || "Support Chat",
      greeting: ds.greeting || "Hi! How can I help you today?",
      position: ds.position === "bottom-left" ? "bottom-left" : "bottom-right",
      chatEndpoint: ds.chatEndpoint || "/api/chat",
    };
  }

  var config = readConfig(scriptEl);

  if (!config.apiUrl) {
    console.error(
      "[ChatbotWidget] Missing required data-api-url attribute on the widget <script> tag. Widget not initialized."
    );
    return;
  }

 function getStoredConversationId() {
    try {
      var existing = window.sessionStorage.getItem(CONVERSATION_STORAGE_KEY);
      if (existing) return existing;

      var newId = generateGuid();
      window.sessionStorage.setItem(CONVERSATION_STORAGE_KEY, newId);
      return newId;
    } catch (e) {
      // storage blocked — fall back to an in-memory id for this page load only
      return generateGuid();
    }
  }

  function generateGuid() {
    if (window.crypto && window.crypto.randomUUID) {
      return window.crypto.randomUUID();
    }
    // fallback for older browsers without crypto.randomUUID
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
      var r = (Math.random() * 16) | 0;
      var v = c === "x" ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  function storeConversationId(id) {
    try {
      window.sessionStorage.setItem(CONVERSATION_STORAGE_KEY, id);
    } catch (e) {
      /* non-fatal: conversation continuity just won't persist across reloads */
    }
  }

  function escapeHtml(str) {
    var div = document.createElement("div");
    div.textContent = str;
    return div.innerHTML;
  }

  function buildStyles(primaryColor) {
    return (
      "\n:host { all: initial; }\n" +
      "* { box-sizing: border-box; font-family: system-ui, -apple-system, Segoe UI, Roboto, sans-serif; }\n" +
      ".widget-root { position: fixed; bottom: 24px; z-index: 2147483000; display: flex; flex-direction: column; align-items: flex-end; }\n" +
      ".widget-root.bottom-right { right: 24px; }\n" +
      ".widget-root.bottom-left { left: 24px; align-items: flex-start; }\n" +
      ".toggle-btn { width: 56px; height: 56px; border-radius: 50%; border: none; cursor: pointer; box-shadow: 0 4px 14px rgba(0,0,0,0.25); background: " +
      primaryColor +
      "; color: #fff; font-size: 26px; display: flex; align-items: center; justify-content: center; margin-top: 12px; }\n" +
      ".toggle-btn:hover { filter: brightness(1.05); }\n" +
      ".panel { width: 340px; max-width: calc(100vw - 32px); height: 460px; max-height: calc(100vh - 120px); background: #fff; border-radius: 14px; box-shadow: 0 10px 30px rgba(0,0,0,0.3); display: flex; flex-direction: column; overflow: hidden; }\n" +
      ".panel-header { background: " +
      primaryColor +
      "; color: #fff; padding: 14px 16px; display: flex; align-items: center; justify-content: space-between; font-weight: 600; font-size: 15px; }\n" +
      ".panel-close { background: none; border: none; color: #fff; font-size: 20px; cursor: pointer; line-height: 1; padding: 0; }\n" +
      ".messages { flex: 1; overflow-y: auto; padding: 14px; display: flex; flex-direction: column; gap: 10px; background: #fafafa; }\n" +
      ".msg { max-width: 82%; padding: 9px 13px; border-radius: 14px; font-size: 14px; line-height: 1.45; white-space: pre-wrap; word-wrap: break-word; }\n" +
      ".msg.user { align-self: flex-end; background: " +
      primaryColor +
      "; color: #fff; border-bottom-right-radius: 4px; }\n" +
      ".msg.bot { align-self: flex-start; background: #eef0f3; color: #1a1a1a; border-bottom-left-radius: 4px; }\n" +
      ".msg.error { align-self: flex-start; background: #fdecea; color: #611a15; }\n" +
      ".msg.typing { align-self: flex-start; background: #eef0f3; color: #6b7280; font-style: italic; }\n" +
      ".input-row { display: flex; gap: 8px; padding: 10px; border-top: 1px solid #e5e7eb; background: #fff; }\n" +
      ".input-row input { flex: 1; border: 1px solid #d1d5db; border-radius: 10px; padding: 10px 12px; font-size: 14px; outline: none; }\n" +
      ".input-row input:focus { border-color: " +
      primaryColor +
      "; }\n" +
      ".input-row button { background: " +
      primaryColor +
      "; color: #fff; border: none; border-radius: 10px; padding: 0 16px; font-size: 14px; cursor: pointer; }\n" +
      ".input-row button:disabled { background: #a0aec0; cursor: not-allowed; }\n" +
      ".footer-note { font-size: 11px; color: #9ca3af; text-align: center; padding: 4px 0 8px; }\n"
    );
  }

  function ChatbotWidget(config) {
    this.config = config;
    this.isOpen = false;
    this.isWaiting = false;
    this.conversationId = getStoredConversationId();
    this.container = null;
    this.shadow = null;
    this.messagesEl = null;
    this.inputEl = null;
    this.sendBtn = null;
    this._init();
  }

  ChatbotWidget.prototype._init = function () {
    this.container = document.createElement("div");
    this.container.id = "systek-chatbot-widget-host";
    document.body.appendChild(this.container);

    this.shadow = this.container.attachShadow({ mode: "open" });

    var style = document.createElement("style");
    style.textContent = buildStyles(this.config.primaryColor);
    this.shadow.appendChild(style);

    this.root = document.createElement("div");
    this.root.className = "widget-root " + this.config.position;
    this.shadow.appendChild(this.root);

    this.toggleBtn = document.createElement("button");
    this.toggleBtn.className = "toggle-btn";
    this.toggleBtn.setAttribute("aria-label", "Open chat");
    this.toggleBtn.textContent = "\uD83D\uDCAC"; // 💬
    this.toggleBtn.addEventListener("click", this._toggle.bind(this));

    this.root.appendChild(this.toggleBtn);

    this._renderGreetingIfEmpty = true;
  };

  ChatbotWidget.prototype._toggle = function () {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this._openPanel();
    } else {
      this._closePanel();
    }
  };

  ChatbotWidget.prototype._openPanel = function () {
    if (this.panel) {
      this.panel.style.display = "flex";
      this.toggleBtn.textContent = "\u00D7"; // ×
      this.toggleBtn.setAttribute("aria-label", "Close chat");
      return;
    }

    this.panel = document.createElement("div");
    this.panel.className = "panel";

    var header = document.createElement("div");
    header.className = "panel-header";

    var titleSpan = document.createElement("span");
    titleSpan.textContent = this.config.title;
    header.appendChild(titleSpan);

    var closeBtn = document.createElement("button");
    closeBtn.className = "panel-close";
    closeBtn.setAttribute("aria-label", "Close chat");
    closeBtn.textContent = "\u00D7";
    closeBtn.addEventListener("click", this._toggle.bind(this));
    header.appendChild(closeBtn);

    this.panel.appendChild(header);

    this.messagesEl = document.createElement("div");
    this.messagesEl.className = "messages";
    this.panel.appendChild(this.messagesEl);

    var inputRow = document.createElement("div");
    inputRow.className = "input-row";

    this.inputEl = document.createElement("input");
    this.inputEl.type = "text";
    this.inputEl.placeholder = "Type your question...";
    this.inputEl.addEventListener("keydown", this._onKeyDown.bind(this));
    inputRow.appendChild(this.inputEl);

    this.sendBtn = document.createElement("button");
    this.sendBtn.textContent = "Send";
    this.sendBtn.addEventListener("click", this._send.bind(this));
    inputRow.appendChild(this.sendBtn);

    this.panel.appendChild(inputRow);

    var footer = document.createElement("div");
    footer.className = "footer-note";
    footer.textContent = "Powered by Systek Bilişim";
    this.panel.appendChild(footer);

    // Insert panel before the toggle button so the button stays visually below it.
    this.root.insertBefore(this.panel, this.toggleBtn);

    this.toggleBtn.textContent = "\u00D7";
    this.toggleBtn.setAttribute("aria-label", "Close chat");

    if (this._renderGreetingIfEmpty && this.config.greeting) {
      this._appendMessage(this.config.greeting, "bot");
      this._renderGreetingIfEmpty = false;
    }

    this.inputEl.focus();
  };

  ChatbotWidget.prototype._closePanel = function () {
    if (this.panel) {
      this.panel.style.display = "none";
    }
    this.toggleBtn.textContent = "\uD83D\uDCAC";
    this.toggleBtn.setAttribute("aria-label", "Open chat");
  };

  ChatbotWidget.prototype._onKeyDown = function (e) {
    if (e.key === "Enter" && !this.isWaiting) {
      this._send();
    }
  };

  ChatbotWidget.prototype._appendMessage = function (text, kind) {
    var el = document.createElement("div");
    el.className = "msg " + kind;
    el.textContent = text; // textContent, never innerHTML — avoids XSS from bot or user content
    this.messagesEl.appendChild(el);
    this.messagesEl.scrollTop = this.messagesEl.scrollHeight;
    return el;
  };

  ChatbotWidget.prototype._send = function () {
    var text = (this.inputEl.value || "").trim();
    if (!text || this.isWaiting) return;

    this.inputEl.value = "";
    this._appendMessage(text, "user");

    this.isWaiting = true;
    this.sendBtn.disabled = true;
    this.inputEl.disabled = true;

    var typingEl = this._appendMessage("Typing...", "typing");

    var self = this;
    var url = this.config.apiUrl + this.config.chatEndpoint;

    fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      sessionId: this.conversationId,
      question: text,
    }),
  })
    .then(function (response) {
      if (!response.ok) {
        throw new Error("Request failed with status " + response.status);
      }
      return response.json();
    })
    .then(function (data) {
      typingEl.remove();
      var reply = (data && data.message) || "Sorry, I didn't get a response. Please try again.";
      self._appendMessage(reply, "bot");
    })
    .catch(function () {
      typingEl.remove();
      self._appendMessage("Sorry, something went wrong. Please try again in a moment.", "error");
    })
    .finally(function () {
      self.isWaiting = false;
      self.sendBtn.disabled = false;
      self.inputEl.disabled = false;
      self.inputEl.focus();
    });
  };

  function start() {
    // Guard against the script being included twice on the same page.
    if (window.__systekChatbotWidgetInstance) return;
    window.__systekChatbotWidgetInstance = new ChatbotWidget(config);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start);
  } else {
    start();
  }
})();
