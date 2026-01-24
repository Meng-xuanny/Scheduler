let selectedJob = null;
let calendar;

// DOM
document.addEventListener("DOMContentLoaded", async function () {
  loadNotifications();

  const calendarEl = document.getElementById("calendar");
  calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: "listWeek",
    headerToolbar: {
      left: "prev,next today",
      center: "title",
      right: "dayGridMonth,timeGridWeek,listWeek",
    },
    eventClick: function (info) {
      const job = info.event;

      selectedJob = {
        id: job.extendedProps.id,
        status: job.extendedProps.status,
      };

      const startTime = new Date(job.start).toLocaleString(undefined, {
        weekday: "short",
        day: "numeric",
        month: "short",
        hour: "numeric",
        minute: "2-digit",
        hour12: true,
      });

      const endTime = job.end
        ? new Date(job.end).toLocaleString(undefined, {
            weekday: "short",
            day: "numeric",
            month: "short",
            hour: "numeric",
            minute: "2-digit",
            hour12: true,
          })
        : "Not completed yet";

      document.getElementById("modalTitle").innerText = job.title;
      document.getElementById("jobDetails").innerHTML = `
            <p><strong>Start:</strong> ${startTime}</p>
            <p><strong>End:</strong> ${endTime}</p>
            <p><strong>Client:</strong> <span class="job-value">${
              job.extendedProps.clientName
            }</span></p>
            <p><strong>Provider:</strong> <span class="job-value">${
              job.extendedProps.providerName
            }</span></p>
            <p><strong>Address:</strong> <span class="job-value">${
              job.extendedProps.address
            }</span></p>
            <p><strong>Notes:</strong> <span class="job-value">${
              job.extendedProps.notes || "N/A"
            }</span></p>
            <p><strong>Status:</strong> <span class="job-value">${
              job.extendedProps.status
            }</span></p>
          `;

      const statusRow = document.getElementById("statusRow");
      const statusCheckbox = document.getElementById("statusCheckbox");

      if (job.extendedProps.status === "Completed") {
        statusRow.style.display = "none";
      } else {
        statusRow.style.display = "block";
      }

      statusCheckbox.checked = job.extendedProps.status === "Completed";

      statusCheckbox.addEventListener("change", async () => {
        if (statusCheckbox.checked) {
          try {
            const res = await fetch(
              `/api/job/${job.extendedProps.id}/complete`,
              {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
              },
            );

            if (!res.ok) throw new Error("Failed to complete job.");

            alert("Job marked as completed.");
            calendar.refetchEvents(); // Reload to show updated status/timing
          } catch (err) {
            console.error("Error updating job status:", err);
            alert("Failed to update job status.");
            statusCheckbox.checked = false; // revert UI change if failed
          }
        } else {
          alert("Jobs cannot be marked incomplete once completed.");
          statusCheckbox.checked = true;
        }
      });

      document.getElementById("jobModal").style.display = "block";
    },
    events: async function (fetchInfo, successCallback, failureCallback) {
      try {
        const res = await fetch("/api/job");
        const data = await res.json();

        const events = data.map((job) => {
          let backgroundColor = "#fbc02d"; // yellow for pending
          if (job.status === "Completed") backgroundColor = "#66bb6a";
          else if (job.status === "Cancelled") backgroundColor = "#ef5350";

          return {
            title: `${job.serviceType} - ${job.clientName}`,
            start: job.startTime,
            end: job.endTime || undefined,
            backgroundColor,
            borderColor: backgroundColor,
            textColor: "#fff",
            extendedProps: {
              id: job.id,
              clientName: job.clientName,
              providerName: job.providerName,
              address: job.address,
              status: job.status,
              notes: job.notes,
            },
          };
        });

        successCallback(events);
      } catch (err) {
        failureCallback(err);
      }
    },
  });

  calendar.render();
  document
    .getElementById("closeModalBtn")
    .addEventListener("click", closeModal);
});

// Reusable Modal Creator
function createModal(htmlContent) {
  const modal = document.createElement("div");
  modal.style.position = "fixed";
  modal.style.top = "50%";
  modal.style.left = "50%";
  modal.style.transform = "translate(-50%, -50%)";
  modal.style.backgroundColor = "white";
  modal.style.padding = "1.5rem";
  modal.style.border = "1px solid #ccc";
  modal.style.borderRadius = "8px";
  modal.style.boxShadow = "0 0 10px rgba(0,0,0,0.3)";
  modal.style.zIndex = "1000";
  modal.style.maxWidth = "400px";
  modal.style.textAlign = "center";
  modal.innerHTML = htmlContent;
  document.body.appendChild(modal);
  return modal;
}

function closeModal() {
  document.getElementById("jobModal").style.display = "none";
}

// Load Notifications
async function loadNotifications() {
  try {
    const res = await fetch("/api/notification");
    const data = await res.json();
    const container = document.getElementById("notificationList");
    container.innerHTML = "";

    if (!data.length) {
      container.innerText = "No notifications available.";
      return;
    }

    data.forEach((note) => {
      if (note.status === "Handled") return;
      renderNotification(note, container);
    });
  } catch (err) {
    document.getElementById("notificationList").innerText =
      "Failed to load notifications.";
    console.error(err);
  }
}

// Render Notifications
function renderNotification(note, container) {
  // Skip if already handled
  if (note.status === "Handled") {
    return;
  }

  const div = document.createElement("div");
  const isQuote = note.message.toLowerCase().includes("quote request");

  div.classList.add(
    "notification",
    isQuote ? "quote-request" : "service-request",
  );

  const messageSpan = document.createElement("span");
  messageSpan.innerHTML = `<strong>${
    isQuote ? "[Quote]" : "[Service]"
  }</strong> ${note.message}`;

  const buttonContainer = document.createElement("div");
  Object.assign(buttonContainer.style, {
    marginTop: "0.5rem",
    display: "flex",
    flexWrap: "wrap",
    gap: "0.5rem",
  });

  const approveBtn = createButton("Approve", "#2e7d32", async () => {
    try {
      // Step 1: Approve the ServiceRequest
      const statusRes = await fetch(
        `/api/request/${note.serviceRequestId}/status`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ status: "Approved" }),
        },
      );

      if (!statusRes.ok) {
        alert("Failed to approve request.");
        return;
      }

      // Step 2: Create the Job
      const jobCreated = await createJob(
        note,
        note.serviceType,
        note.preferredDate,
      );

      if (!jobCreated) {
        alert("Failed to create job.");
        return;
      }

      // Step 3: Mark the ProviderNotification as "Handled"
      const notifRes = await fetch(`/api/notification/${note.id}/status`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify("Handled"),
      });

      if (!notifRes.ok) {
        alert("Job created, but failed to mark notification as handled.");
      }

      // Step 4: UI Updates
      alert("Job created!");
      if (div) div.remove();
      calendar.refetchEvents();
    } catch (err) {
      console.error("Error approving request:", err);
      alert("Something went wrong while approving request.");
    }
  });

  const rearrangeBtn = createButton("Re-arrange", "#c62828", () => {
    showContactClientModal({
      // clientPhone: note.clientPhone,
      onConfirmed: () => showRearrangeForm(note, div),
    });
  });

  // Badge
  const badge = document.createElement("span");
  badge.textContent = note.status === "Unread" ? "🔔 Unread" : "✅ Handled";
  badge.style = `
      font-size: 0.8rem;
      background-color: ${note.status === "Unread" ? "#ff9800" : "#81c784"};
      color: white;
      padding: 2px 6px;
      border-radius: 4px;
      margin-left: 0.5rem;
    `;
  messageSpan.appendChild(badge);

  // if (note.status === 'Unread') {
  //   div.style.border = '2px solid #fbc02d';
  //   div.style.backgroundColor = '#fffde7';
  // }

  div.addEventListener("mouseenter", () => {
    div.style.boxShadow = "0 4px 10px rgba(0,0,0,0.2)";
  });

  div.addEventListener("mouseleave", () => {
    div.style.boxShadow = "";
  });

  buttonContainer.append(approveBtn, rearrangeBtn);
  div.append(messageSpan, buttonContainer);
  container.appendChild(div);
}

function createButton(label, bgColor, onClick) {
  const button = document.createElement("button");
  button.textContent = label;
  button.style.backgroundColor = bgColor;
  button.style.color = "white";
  button.style.border = "none";
  button.style.padding = "0.5rem 1rem";
  button.style.margin = "0.25rem";
  button.style.borderRadius = "4px";
  button.style.cursor = "pointer";
  button.addEventListener("click", onClick);
  return button;
}

// Rearrange Form
function showRearrangeForm(note, div) {
  const modal = createModal(`
  <form id="rearrangeForm">
    <button id="closeModalBtn" style="position:absolute;top:10px;right:10px;background:transparent;border:none;font-size:1.2rem;color:#555;cursor:pointer;">✖</button>
    <label>Service Type:</label>
    <input type="text" id="newServiceType" style="width:100%;margin-bottom:1rem;" value=${note.serviceType} />
    <label>Preferred Date & Time:</label>
    <input type="datetime-local" id="newDate" style="width:100%;margin-bottom:1rem;" />
    <button id="submitRearrangeBtn" style="background-color:#2e7d32;color:white;padding:0.5rem 1rem;border:none;border-radius:4px;">Submit</button>
  </form>
  `);

  modal.querySelector("#closeModalBtn").onclick = () =>
    document.body.removeChild(modal);

  modal.querySelector("#rearrangeForm").onsubmit = async (e) => {
    e.preventDefault(); // Prevent form reload

    const newServiceType = document.getElementById("newServiceType").value;
    const newDate = document.getElementById("newDate").value;

    if (!newServiceType || !newDate) {
      alert("Please fill in all fields.");
      return;
    }

    try {
      // Step 1: Update ServiceRequest status
      const statusRes = await fetch(
        `/api/request/${note.serviceRequestId}/status`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ status: "Re-arranged" }),
        },
      );

      if (!statusRes.ok) {
        alert("Failed to update request status.");
        return;
      }

      // Step 2: Create the Job
      const jobCreated = await createJob(note, newServiceType, newDate);

      if (!jobCreated) {
        alert("Failed to create job.");
        return;
      }

      // Step 3: Update provider notification to Handled
      const notifRes = await fetch(`/api/notification/${note.id}/status`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify("Handled"),
      });

      if (!notifRes.ok) {
        alert("Job created, but failed to update notification.");
      }

      // Step 4: Clean up UI
      alert("Job created!");
      document.body.removeChild(modal);
      if (div) div.remove();
      calendar.refetchEvents();
    } catch (err) {
      console.error(err);
      alert("Something went wrong while rearranging.");
    }
  };
}

function showContactClientModal({ clientPhone, onConfirmed }) {
  const modal = createModal(`
    <button id="closeModalBtn" style="position:absolute;top:10px;right:10px;background:transparent;border:none;font-size:1.2rem;color:#555;cursor:pointer;">✖</button>
    <p><strong style="color:red;">You must contact the client to re-arrange a service.</strong></p>
    <p>Have you contacted the client?</p>
    <div style="margin-top:1rem;">
      <button id="confirmContactBtn" style="margin-right:0.5rem;background-color:#407f46;color:#fff;padding:0.5rem 1rem;border:none;border-radius:4px;cursor:pointer;">Yes, I’ve contacted</button>
      <button id="showPhoneBtn" style="background-color:#1976d2;color:#fff;padding:0.5rem 1rem;border:none;border-radius:4px;cursor:pointer;">Show contact info</button>
    </div>
    <div id="phoneInfo" style="margin-top:1rem;color:#555;"></div>
  `);

  modal.querySelector("#closeModalBtn").onclick = () =>
    document.body.removeChild(modal);
  modal.querySelector("#showPhoneBtn").onclick = () => {
    modal.querySelector("#phoneInfo").innerText = `📞 ${
      clientPhone || "No number available"
    }`;
  };
  modal.querySelector("#confirmContactBtn").onclick = () => {
    document.body.removeChild(modal);
    if (onConfirmed) onConfirmed();
  };
}

// Job functions
async function createJob(note, serviceType, preferredDate) {
  try {
    const jobData = {
      serviceRequestId: note.serviceRequestId,
      serviceType: serviceType,
      providerName: "Jaydn",
      address: note.address,
      startTime: preferredDate,
      endTime: null,
      notes: note.notes || "",
      status: "Scheduled",
    };

    const res = await fetch("/api/job", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(jobData),
    });

    if (!res.ok) {
      alert("Failed to create job.");
      return false;
    }

    return true;
  } catch (err) {
    console.error("Error creating job:", err);
    alert("Something went wrong while creating the job.");
    return false;
  }
}
