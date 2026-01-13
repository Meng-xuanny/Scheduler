function switchSection(section) {
  document
    .querySelectorAll(".dashboard-section")
    .forEach((s) => s.classList.add("hidden"));
  document.getElementById(section).classList.remove("hidden");
}

async function loadData(
  url,
  tableId,
  filterKeys = [],
  filterInputIds = [],
  booleanFilters = [],
  extraFilters = {}
) {
  const res = await fetch(url);
  const data = await res.json();
  const table = document.getElementById(tableId);
  table.innerHTML = "";

  if (!data.length) {
    table.innerHTML = "<tr><td>No records found.</td></tr>";
    return;
  }

  const headers = Object.keys(data[0]);
  const thead = document.createElement("thead");
  const headerRow = document.createElement("tr");
  headers.forEach((h) => {
    const th = document.createElement("th");
    th.innerText = h;
    headerRow.appendChild(th);
  });
  thead.appendChild(headerRow);
  table.appendChild(thead);

  const renderRows = (records) => {
    const tbody = document.createElement("tbody");
    records.forEach((record) => {
      const tr = document.createElement("tr");
      headers.forEach((key) => {
        const td = document.createElement("td");
        td.innerText = record[key] ?? "";
        tr.appendChild(td);
      });
      tbody.appendChild(tr);
    });
    return tbody;
  };

  let currentData = [...data];
  table.appendChild(renderRows(currentData));

  const applyFilters = () => {
    currentData = data.filter((item) => {
      // Text filters
      const textMatches = filterKeys.every((key, i) => {
        const val =
          document
            .getElementById(filterInputIds[i])
            ?.value.trim()
            .toLowerCase() || "";
        return (item[key] || "").toString().toLowerCase().includes(val);
      });

      // Boolean filters
      const boolMatches = booleanFilters.every(({ id, key }) => {
        const checkbox = document.getElementById(id);
        return checkbox?.checked ? item[key] === true : true;
      });

      // Dropdown filters
      const dropdownMatches = (extraFilters.dropdowns || []).every(
        ({ id, key }) => {
          const select = document.getElementById(id);
          return select?.value ? item[key] === select.value : true;
        }
      );

      return textMatches && boolMatches && dropdownMatches;
    });

    const newTbody = renderRows(currentData);
    table.replaceChild(newTbody, table.querySelector("tbody"));
  };

  // Attach input listeners for text filters
  filterInputIds.forEach((inputId) => {
    document.getElementById(inputId)?.addEventListener("input", applyFilters);
  });

  // Attach listeners for boolean filters
  booleanFilters.forEach(({ id }) => {
    document.getElementById(id)?.addEventListener("change", applyFilters);
  });

  // Dropdown listeners
  (extraFilters.dropdowns || []).forEach(({ id }) => {
    document.getElementById(id)?.addEventListener("change", applyFilters);
  });
}

document.addEventListener("DOMContentLoaded", () => {
  loadData(
    "/api/client",
    "clientsTable",
    ["fullName", "servicePreference"],
    ["filterByClientName", "filterByPreference"]
  );
  loadData(
    "/api/provider",
    "providersTable",
    ["fullName", "skills"],
    ["filterByProviderName", "filterBySkills"],
    [
      { id: "filterByAvailability", key: "isAvailable" }, // Boolean filter
    ]
  );
  loadData(
    "/api/request",
    "requestsTable",
    ["clientName", "providerName", "preferredDate"],
    ["filterClientInput", "filterProviderInput", "filterDateInput"]
  );
  loadData(
    "/api/notification",
    "notificationsTable",
    ["serviceRequestId"],
    ["filterByRequest"],
    [],
    {
      dropdowns: [{ id: "filterByNotificationStatus", key: "status" }],
    }
  );

  loadData(
    "/api/job",
    "jobsTable",
    ["clientName", "providerName", "startTime"],
    ["filterJobByClient", "filterJobByProvider", "filterJobByDate"],
    [{ id: "filterByClientConfirmation", key: "isClientConfirmed" }],
    { dropdowns: [{ id: "filterByJobStatus", key: "status" }] }
  );
});

// document.getElementById('filterClientInput').addEventListener('input', async (e) => {
//   const query = e.target.value;
//   const res = await fetch(`/api/client/search?name=${encodeURIComponent(query)}`);
//   const clients = await res.json();
//   // show suggestions or filter service request table
// });
