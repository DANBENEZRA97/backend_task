const BASE_URL = "https://localhost:7038/api";

let eventsVisible = false;

function toggleEvents() {
    const ul = document.getElementById("eventsList");

    if (eventsVisible) {
        ul.innerHTML = "";
        eventsVisible = false;
    } else {
        getAllEvents();
        eventsVisible = true;
    }
}

function getAllEvents() {
    fetch(`${BASE_URL}/Event/by-dates?from=2000-01-01&to=2100-01-01`)
        .then(res => res.json())
        .then(events => {
            const ul = document.getElementById("eventsList");
            ul.innerHTML = "";
            events.forEach(event => {
                fetch(`${BASE_URL}/Event/${event.id}/registration`)
                    .then(res => res.json())
                    .then(registrations => {
                        const li = document.createElement("li");
                        const mapsUrl = `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(event.location)}`;

                        li.innerHTML = `
                            <strong>ID: ${event.id}</strong><br>
                            <strong>${event.name}</strong> (${event.startDate} → ${event.endDate}) - ${event.location}
                            <br>Max Registrations: ${event.maxRegistrations}
                            <br>Registered Users: ${registrations.length}
                            <br><a href="${mapsUrl}" target="_blank">📍 Open in Google Maps</a>
                        `;
                        ul.appendChild(li);
                    });
            });
        })
        .catch(err => alert("Failed to load events."));
}


function submitEvent() {
    const newEvent = {
        name: document.getElementById("eventName").value.trim(),
        location: document.getElementById("eventLocation").value.trim(),
        startDate: document.getElementById("eventStart").value,
        endDate: document.getElementById("eventEnd").value,
        maxRegistrations: parseInt(document.getElementById("eventMax").value)
    };

    

 

    fetch(`${BASE_URL}/Event`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newEvent)
    })
        .then(res => {
            if (res.ok) {
                alert("New event succesfully added :) .");
                if (eventsVisible) getAllEvents();
                document.getElementById("eventName").value = "";
                document.getElementById("eventLocation").value = "";
                document.getElementById("eventStart").value = "";
                document.getElementById("eventEnd").value = "";
                document.getElementById("eventMax").value = "";

            }
            else {
                alert("Event not created,try again.");
            }
        })
               
        .catch(() => alert("Error creating event."));
}

function updateEvent() {
    const id = parseInt(document.getElementById("updateId").value);
    const updatedEvent = {
        name: document.getElementById("updateName").value,
        location: document.getElementById("updateLocation").value,
        startDate: document.getElementById("updateStart").value,
        endDate: document.getElementById("updateEnd").value,
        maxRegistrations: parseInt(document.getElementById("updateMax").value)
    };

    fetch(`${BASE_URL}/Event/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(updatedEvent)
    })
        .then(res => {
            if (!res.ok) throw new Error("Update failed");
            return res.json();
        })
        .then(() => {
            alert("Event updated successfully.");
            getAllEvents();
        })
        .catch(err => alert(err.message));
}

function deleteEventById() {
    const id = parseInt(document.getElementById("deleteEventId").value);

    fetch(`${BASE_URL}/Event/by_id?id=${id}`, {
        method: "DELETE"
    })
        .then(res => {
            if (res.status === 204) {
                alert("Event deleted.");
                getAllEvents();
            } else {
                alert("Event not found.");
            }
        })
        .catch(() => alert("Delete failed."));
}

function getWeather() {
    const id = document.getElementById("weatherEventId").value;
    fetch(`${BASE_URL}/Event/weatherbyeventid?id=${id}`)
        .then(res => res.json())
        .then(data => {
            document.getElementById("weatherResult").textContent = JSON.stringify(data, null, 2);
        })
        .catch(() => alert("Weather fetch failed."));
}

function registerUser() {
    const userId = document.getElementById("registerUserId").value;
    const eventId = document.getElementById("registerEventId").value;

    fetch(`${BASE_URL}/EventUser?userId=${userId}&eventId=${eventId}`, {
        method: "POST"
    })
        .then(res => res.text())
        .then(msg => {
            document.getElementById("registerMessage").textContent = msg;
        })
        .catch(() => alert("Registration failed."));
}
window.submitEvent = submitEvent;