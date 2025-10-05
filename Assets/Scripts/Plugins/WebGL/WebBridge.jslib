mergeInto(LibraryManager.library, {
  WebBridge_SendScore: function (v) {
    // v is a number (int) from Unity
    if (typeof window.onUnityScore === "function") {
      window.onUnityScore(v);
    } else {
      console.log("[Unity->JS] score =", v);
    }
  }
});
