// Auth state control
const AuthState = {
  INIT: "INIT",
  LOADING: "LOADING",
  AUTHENTICATED: "AUTHENTICATED",
  UNAUTHENTICATED: "UNAUTHENTICATED",
};

let authState = AuthState.INIT;

function setAuthState(newState) {
  authState = newState;
  renderAuthState();
}

function renderAuthState() {
  // Hide everything first
  document.querySelectorAll("main > .section").forEach((el) => {
    el.style.display = "none";
  });

  switch (authState) {
    case AuthState.LOADING:
      document.getElementById("loadingSection").style.display = "block";
      break;

    case AuthState.AUTHENTICATED:
      document
        .querySelectorAll(
          "main > .section:not(#authSection):not(#signUpSection):not(#loadingSection)"
        )
        .forEach((el) => (el.style.display = "block"));
      break;

    case AuthState.UNAUTHENTICATED:
      document.getElementById("authSection").style.display = "block";
      break;
  }
}

//Initial loading
document.addEventListener("DOMContentLoaded", async () => {
  const clientId = localStorage.getItem("clientId");

  if (!clientId) {
    setAuthState(AuthState.UNAUTHENTICATED);
    return;
  }

  setAuthState(AuthState.LOADING);

  const ok = await enableMainApp(clientId);

  setAuthState(ok ? AuthState.AUTHENTICATED : AuthState.UNAUTHENTICATED);
});

// Sign In
document.getElementById("signInForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const form = e.target;
  const email = form.email.value;
  const phone = form.phoneNumber.value;
  const name = form.fullName.value;

  try {
    const res = await fetch(
      `/api/client/lookup?email=${email}&phone=${phone}&name=${name}`
    );
    if (!res.ok) return alert("Client not found. Please check your details.");

    const client = await res.json();
    localStorage.setItem("clientId", client.id);

    const ok = await enableMainApp(client.id);
    setAuthState(ok ? AuthState.AUTHENTICATED : AuthState.UNAUTHENTICATED);
  } catch (err) {
    console.error(err);
    alert("Error signing in.");
  }
});

// Sign Up
document.getElementById("signUpForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const form = e.target;
  const client = {
    email: form.email.value,
    phoneNumber: form.phoneNumber.value,
    fullName: form.fullName.value,
    address: form.address.value,
    servicePreference: form.servicePreference.value,
  };

  try {
    const res = await fetch("/api/client/signup", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(client),
    });

    if (!res.ok) return alert("Failed to sign up.");

    const created = await res.json();
    localStorage.setItem("clientId", created.id);

    // Wait for enableMainApp to finish
    const ok = await enableMainApp(created.id);
    setAuthState(ok ? AuthState.AUTHENTICATED : AuthState.UNAUTHENTICATED);
  } catch (err) {
    console.error(err);
    alert("Error signing up.");
  }
});

document.getElementById("logoutBtn")?.addEventListener("click", logout);

function logout() {
  localStorage.removeItem("clientId");
  setAuthState(AuthState.UNAUTHENTICATED);
}

async function enableMainApp(clientId) {
  try {
    const client = await fetchClientProfile(clientId);

    if (!client || !client.fullName) {
      throw new Error("Invalid client");
    }
    localStorage.setItem("clientId", clientId);

    loadProviders();
    fetchRequestedServices(clientId);
    loadScheduledServices(clientId);

    return true;
  } catch {
    localStorage.removeItem("clientId");
    return false;
  }
}

// Auth
document.addEventListener("DOMContentLoaded", () => {
  const toSignUp = document.getElementById("goToSignUp");
  const toSignIn = document.getElementById("goToSignIn");

  if (toSignUp) {
    toSignUp.addEventListener("click", (e) => {
      e.preventDefault();
      toggleAuthForms(true);
    });
  }

  if (toSignIn) {
    toSignIn.addEventListener("click", (e) => {
      e.preventDefault();
      toggleAuthForms(false);
    });
  }
});

function toggleAuthForms(isSigningUp) {
  const signIn = document.getElementById("authSection");
  const signUp = document.getElementById("signUpSection");

  signUp.style.display = isSigningUp ? "block" : "none";
  signIn.style.display = isSigningUp ? "none" : "block";
}

// Profile
async function fetchClientProfile(clientId) {
  const res = await fetch(`/api/client/${clientId}`);
  if (!res.ok) {
    throw new Error("Client not found");
  }
  const client = await res.json();
  renderClientProfile(client);
  return client;
}

function renderClientProfile(client) {
  const displayHtml = `
    <p><strong>Name:</strong> <span id="fullName">${client.fullName}</span></p>
    <p><strong>Age:</strong> <span id="age">${client.age ?? ""}</span></p>
    <p><strong>Email:</strong> <span id="email">${client.email}</span></p>
    <p><strong>Phone:</strong> <span id="phoneNumber">${
      client.phoneNumber
    }</span></p>
    <p><strong>Address:</strong> <span id="address">${client.address}</span></p>
    <p><strong>Occupation:</strong> <span id="work">${
      client.work ?? ""
    }</span></p>
    <p><strong>Hobby:</strong> <span id="hobby">${client.hobby ?? ""}</span></p>
    <p><strong>Service Preference:</strong> <span id="servicePreference">${
      client.servicePreference ?? ""
    }</span></p>
  `;
  document.getElementById("clientProfile").innerHTML = displayHtml;
  document.getElementById("editProfileBtn").style.display = "inline-block";

  // 2. Pre-fill request form fields if they exist
  const quoteFormAddress = document.querySelector(
    'input[name="quote-address"]'
  );
  const serviceFormAddress = document.getElementById("service-address");

  if (quoteFormAddress && client.address) {
    quoteFormAddress.value = client.address;
  }

  if (serviceFormAddress && client.address) {
    serviceFormAddress.value = client.address;
  }
}

document.addEventListener("DOMContentLoaded", () => {
  const editBtn = document.getElementById("editProfileBtn");
  if (editBtn) {
    editBtn.addEventListener("click", enableEditProfile);
  }
});

function enableEditProfile() {
  const fields = [
    "fullName",
    "age",
    "email",
    "phoneNumber",
    "address",
    "work",
    "hobby",
    "servicePreference",
  ];

  fields.forEach((id) => {
    const span = document.getElementById(id);
    const value = span.textContent.trim();
    span.outerHTML = `<input type="text" id="${id}" value="${value}" />`;
  });

  document.getElementById("editProfileBtn").style.display = "none";

  const saveBtn = document.createElement("button");
  saveBtn.innerText = "Save Changes";
  saveBtn.onclick = saveProfileChanges;
  saveBtn.id = "saveProfileBtn";
  saveBtn.style.marginRight = "0.5rem";

  const cancelBtn = document.createElement("button");
  cancelBtn.innerText = "Cancel";
  cancelBtn.onclick = () => {
    document.getElementById("saveProfileBtn")?.remove();
    document.getElementById("cancelProfileBtn")?.remove();
    fetchClientProfile(); // Re-fetch original data and render as text
  };
  cancelBtn.id = "cancelProfileBtn";

  const container = document.getElementById("clientProfile");
  container.appendChild(saveBtn);
  container.appendChild(cancelBtn);
}

async function saveProfileChanges() {
  const clientId = localStorage.getItem("clientId");

  const updated = {
    id: clientId,
    fullName: document.getElementById("fullName")?.value,
    age: parseInt(document.getElementById("age")?.value) || null,
    email: document.getElementById("email")?.value,
    phoneNumber: document.getElementById("phoneNumber")?.value,
    address: document.getElementById("address")?.value,
    work: document.getElementById("work")?.value,
    hobby: document.getElementById("hobby")?.value,
    servicePreference: document.getElementById("servicePreference")?.value,
  };

  try {
    const res = await fetch(`/api/client/${clientId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(updated),
    });

    if (!res.ok) throw new Error("Failed to update");

    alert("Profile updated.");
    document.getElementById("saveProfileBtn")?.remove();
    fetchClientProfile(clientId); // Re-render text view
  } catch (err) {
    console.error(err);
    alert("Error saving profile.");
  }
}

// Provider
async function loadProviders() {
  try {
    const res = await fetch(`/api/provider`);
    if (!res.ok) throw new Error("Response not OK");

    const providers = await res.json();

    const dropdowns = document.querySelectorAll(".providerDropdown");
    dropdowns.forEach((select) => {
      select.innerHTML = '<option value="">Select a provider</option>'; // reset dropdown
      providers.forEach((provider) => {
        const option = document.createElement("option");
        option.value = provider.id;
        option.textContent = provider.fullName;
        select.appendChild(option);
      });
    });
  } catch (err) {
    console.error("Failed to load providers:", err);
  }
}

//  Quote
document.addEventListener("DOMContentLoaded", () => {
  const quoteBtn = document.getElementById("quoteBtn");
  const cancelBtn = document.getElementById("cancelQuoteBtn");

  if (quoteBtn) quoteBtn.addEventListener("click", openQuoteModal);
  if (cancelBtn) cancelBtn.addEventListener("click", closeQuoteModal);
});

// Open the quote modal with preset logic
function openQuoteModal() {
  const clientId = localStorage.getItem("clientId");
  fetchClientProfile(clientId);

  openRequestModal({
    clientId,
    title: "Request a Free Quote",
    isQuote: true,
    onSubmit: async (data) => {
      data.notes = "Quote requested.";
      await submitServiceRequest(data, () => {
        fetchRequestedServices(clientId);
      });
    },
  });
}

function openRequestModal({
  clientId,
  title,
  defaultType = "",
  defaultDate = "",
  defaultNotes = "",
  onSubmit,
}) {
  const modal = document.getElementById("quoteModal");
  const form = document.getElementById("quoteForm");

  // Set title and default values
  modal.querySelector("h3").innerText = title;
  form.serviceType.value = defaultType;
  form.preferredDate.value = defaultDate;

  loadProviders();

  // Replace the form submission
  form.onsubmit = async (e) => {
    e.preventDefault();

    const data = {
      clientId,
      serviceType: form.serviceType.value,
      preferredDate: form.preferredDate.value,
      providerId: form.providerId.value,
      notes: form.notes?.value || "",
      isQuote: title.includes("Quote"),
    };

    await onSubmit(data);
    form.reset();
    closeQuoteModal();
  };

  // Show modal
  modal.style.display = "block";
}

function closeQuoteModal() {
  document.getElementById("quoteModal").style.display = "none";
}

// Job Modal
document.addEventListener("DOMContentLoaded", () => {
  const closeJobModalBtn = document.getElementById("closeJobModalBtn");
  if (closeJobModalBtn) {
    closeJobModalBtn.addEventListener("click", closeJobModal);
  }
});

function closeJobModal() {
  document.getElementById("jobModal").style.display = "none";
}

function addShowMoreToggle(container, items, renderItem, maxVisible = 5) {
  container.innerHTML = ""; // clear container first
  let isExpanded = false;

  const toggleBtn = document.createElement("button");
  toggleBtn.textContent = "Show more";
  toggleBtn.style = `
    background: none;
    border: none;
    color: #1976d2;
    text-decoration: underline;
    cursor: pointer;
    margin-top: 0.5rem;
  `;

  const renderItems = () => {
    container.innerHTML = ""; // Clear list
    const toRender = isExpanded ? items : items.slice(0, maxVisible);
    toRender.forEach((item) => {
      const element = renderItem(item);
      container.appendChild(element);
    });

    if (items.length > maxVisible) {
      container.appendChild(toggleBtn);
    }
  };

  toggleBtn.onclick = () => {
    isExpanded = !isExpanded;
    toggleBtn.textContent = isExpanded ? "Show less" : "Show more";
    renderItems();
  };

  renderItems(); // Initial render
}

// Service request
document
  .getElementById("serviceRequestForm")
  .addEventListener("submit", async (e) => {
    e.preventDefault();
    const form = e.target;
    const clientId = localStorage.getItem("clientId");

    const data = {
      clientId: clientId,
      serviceType: form.serviceType.value,
      providerId: form.providerId.value,
      preferredDate: form.preferredDate.value,
      notes: form.notes.value,
    };

    await submitServiceRequest(data, () => {
      form.reset();
      fetchRequestedServices(clientId);
    });
  });

async function fetchRequestedServices(clientId) {
  const res = await fetch(`/api/request/client/${clientId}`);
  const requests = (await res.json()).reverse();

  const list = document.getElementById("requestedServices");
  list.innerHTML = ""; // Clear previous entries

  const renderRequestItem = (r) => {
    const item = document.createElement("li");
    item.className = "service-item";
    item.textContent = `${r.serviceType} (Preferred: ${new Date(
      r.preferredDate
    ).toLocaleDateString()}) - ${r.status}`;
    return item;
  };

  addShowMoreToggle(list, requests, renderRequestItem, 5);
}

async function submitServiceRequest(data, onSuccess) {
  try {
    const res = await fetch("/api/request", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });

    if (res.ok) {
      alert("Request submitted!");
      onSuccess?.();
    } else {
      alert("Failed to submit request.");
    }
  } catch (err) {
    console.error(err);
    alert("Error submitting request.");
  }
}

// scheduled services
async function loadScheduledServices(clientId) {
  const list = document.getElementById("scheduledServices");
  list.innerHTML = "Loading...";

  try {
    const res = await fetch(`/api/job/client/${clientId}`);
    if (!res.ok) throw new Error("Failed to fetch jobs");

    const jobs = (await res.json()).reverse();

    if (!jobs.length) {
      list.innerHTML = "<li>No scheduled services found.</li>";
      return;
    }

    const renderJobItem = (job) => {
      const li = document.createElement("li");
      li.classList.add("service-item");
      li.style.cursor = "pointer";
      li.style.padding = "1rem";
      li.style.borderRadius = "6px";
      li.style.marginBottom = "0.75rem";
      li.style.color = "#fff";

      let statusColor;
      if (job.isClientConfirmed) statusColor = "#66bb6a";
      else if (job.status === "Completed") statusColor = "#42a5f5";
      else if (job.status === "Cancelled") statusColor = "#ef5350";
      else if (job.status === "Scheduled") statusColor = "#fbc02d";
      else statusColor = "#90a4ae";

      li.style.backgroundColor = statusColor;

      const start = new Date(job.startTime).toLocaleString();
      li.innerHTML = `
        <div>
          <strong>${job.serviceType}</strong><br/>
          <small>${start}</small>
        </div>
      `;
      li.onclick = () => showJobModal(job);
      return li;
    };

    addShowMoreToggle(list, jobs, renderJobItem, 5);
  } catch (err) {
    console.error(err);
    list.innerHTML = "<li>Error loading scheduled services.</li>";
  }
}

function showJobModal(job) {
  const modal = document.getElementById("jobModal");
  const title = document.getElementById("modalTitle");
  const detail = document.getElementById("jobDetails");
  const actions = document.getElementById("jobActions");

  title.innerText = `${job.serviceType}`;
  detail.innerHTML = `
    <p><strong>Start:</strong> ${new Date(job.startTime).toLocaleString()}</p>
    <p><strong>End:</strong> ${
      job.endTime ? new Date(job.endTime).toLocaleString() : "N/A"
    }</p>
    <p><strong>Client:</strong> ${job.clientName}</p>
    <p><strong>Provider:</strong> ${job.providerName}</p>
    <p><strong>Address:</strong> ${job.address}</p>
    <p><strong>Notes:</strong> ${job.notes || "N/A"}</p>
    <p><strong>Status:</strong> ${job.status}</p>
    <p><strong>Client Confirmed:</strong> ${
      job.isClientConfirmed ? "Yes" : "No"
    }</p>
  `;

  actions.innerHTML = "";

  // ✅ Confirm button
  if (!job.isClientConfirmed && job.status !== "Cancelled") {
    const confirmBtn = document.createElement("button");
    confirmBtn.innerText = "Confirm Job Completed";
    confirmBtn.style = buttonStyle("#2e7d32");
    confirmBtn.onclick = async () => {
      const clientId = localStorage.getItem("clientId");
      await confirmJob(job.id);
      modal.style.display = "none";
      loadScheduledServices(clientId);
    };
    actions.appendChild(confirmBtn);
  }

  // 🔁 Reschedule button
  if (job.status === "Scheduled" && !job.isClientConfirmed) {
    const rescheduleBtn = document.createElement("button");
    rescheduleBtn.innerText = "Reschedule";
    rescheduleBtn.style = buttonStyle("#1976d2");
    rescheduleBtn.onclick = () => {
      modal.style.display = "none";
      rescheduleJob(job);
    };
    actions.appendChild(rescheduleBtn);
  }

  // ❌ Cancel button
  if (!job.isClientConfirmed && job.status !== "Cancelled") {
    const cancelBtn = document.createElement("button");
    cancelBtn.innerText = "Cancel Job";
    cancelBtn.style = buttonStyle("#c62828");
    cancelBtn.onclick = async () => {
      const clientId = localStorage.getItem("clientId");
      await cancelJob(job.id);
      modal.style.display = "none";
      loadScheduledServices(clientId);
    };
    actions.appendChild(cancelBtn);
  }

  modal.style.display = "block";
}

function buttonStyle(color) {
  return `
    background-color: ${color};
    color: white;
    padding: 0.5rem 1rem;
    border: none;
    margin: 0.5rem;
    border-radius: 4px;
    cursor: pointer;
  `;
}

async function confirmJob(jobId) {
  const res = await fetch(`/api/job/${jobId}/confirm`, { method: "PUT" });
  return res.ok ? alert("Job confirmed!") : alert("Failed to confirm.");
}

async function cancelJob(jobId, skipConfirm = false, cancelReason = "") {
  if (!skipConfirm) {
    const confirmCancel = confirm("Are you sure you want to cancel this job?");
    if (!confirmCancel) return;

    // Only ask if reason wasn't passed in
    cancelReason = prompt("Please provide a reason for cancellation:")?.trim();
  }

  if (!cancelReason) {
    alert("Cancellation aborted. A reason is required.");
    return;
  }

  const res = await fetch(`/api/job/${jobId}/status`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      status: "Cancelled",
      notes: cancelReason,
    }),
  });

  if (!res.ok) {
    alert("Failed to cancel the job.");
  }
}

async function rescheduleJob(job) {
  const clientId = localStorage.getItem("clientId");
  const modal = document.createElement("div");
  modal.style = `
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background: white;
    padding: 1.5rem;
    border-radius: 8px;
    box-shadow: 0 0 10px rgba(0,0,0,0.3);
    z-index: 1000;
    max-width: 400px;
  `;

  modal.innerHTML = `
    <h3 style="margin-top: 0;">Reason for Rescheduling</h3>
    <label>
      Select a reason:
      <select id="reasonDropdown" style="width:100%;margin:0.5rem 0;">
        <option value="">-- Please choose an option --</option>
        <option value="Not available at scheduled time">Not available at scheduled time</option>
        <option value="Need a different service">Need a different service</option>
        <option value="Need a different provider">Need a different service provider</option>
        <option value="Other">Other</option>
      </select>
    </label>
    <textarea id="otherReason" placeholder="If 'Other', please specify..." style="width:100%;margin-bottom:1rem;display:none;"></textarea>
    <div style="text-align:right;">
      <button id="cancelBtn" style="margin-right:0.5rem;">Cancel</button>
      <button id="confirmBtn" style="background-color:#1976d2;color:white;padding:0.5rem 1rem;border:none;border-radius:4px;">Continue</button>
    </div>
  `;

  document.body.appendChild(modal);

  const dropdown = modal.querySelector("#reasonDropdown");
  const otherTextarea = modal.querySelector("#otherReason");
  const confirmBtn = modal.querySelector("#confirmBtn");
  const cancelBtn = modal.querySelector("#cancelBtn");

  dropdown.onchange = () => {
    otherTextarea.style.display = dropdown.value === "Other" ? "block" : "none";
  };

  cancelBtn.onclick = () => {
    document.body.removeChild(modal);
  };

  confirmBtn.onclick = () => {
    const selected = dropdown.value;
    const reason = selected === "Other" ? otherTextarea.value.trim() : selected;

    if (!reason) {
      alert("Please select or enter a reason.");
      return;
    }

    document.body.removeChild(modal);

    openRequestModal({
      clientId,
      title: "Reschedule Service",
      defaultType: job.serviceType,
      defaultDate: "",
      defaultNotes: reason,
      onSubmit: async (data) => {
        data.notes = data.notes || "Reschedule requested.";
        data.providerId = job.providerId;

        await cancelJob(job.id, true, reason);
        await submitServiceRequest(data, () => {
          loadScheduledServices(clientId);
          fetchRequestedServices(clientId);
          alert("Job rescheduled successfully!");
        });
      },
    });
  };
}
