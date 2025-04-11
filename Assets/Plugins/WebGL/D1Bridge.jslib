mergeInto(LibraryManager.library, {
    SendDataToD1: function (jsonData) {
        let data = UTF8ToString(jsonData);

        fetch("https://cloudflare-api.tarynthompson349.workers.dev/submit", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: data
        })
        .then(response => response.json())
        .then(result => console.log("Success:", result))
        .catch(error => console.error("Error:", error));
    }
});