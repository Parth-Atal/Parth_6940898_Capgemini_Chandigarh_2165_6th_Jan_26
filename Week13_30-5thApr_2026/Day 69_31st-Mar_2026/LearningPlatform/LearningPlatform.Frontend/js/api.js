// ============================================================
//  api.js — Centralized API Client
//  Base URL: adjust to match your backend
// ============================================================
const API_BASE = "http://localhost:5000";

function getToken() {
  return localStorage.getItem("jwt_token");
}

function buildHeaders(includeAuth = true) {
  const headers = { "Content-Type": "application/json" };
  if (includeAuth) {
    const token = getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
  }
  return headers;
}

async function handleResponse(response) {
  const text = await response.text();
  let data;
  try {
    data = JSON.parse(text);
  } catch {
    data = { message: text };
  }

  if (!response.ok) {
    // Handle server validation errors
    const message =
      data?.Error ||
      data?.error ||
      data?.title ||
      data?.message ||
      (data?.errors ? Object.values(data.errors).flat().join(", ") : null) ||
      `Request failed with status ${response.status}`;
    throw new Error(message);
  }
  return data;
}

async function apiGet(path) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "GET",
    headers: buildHeaders(),
  });
  return handleResponse(response);
}

async function apiPost(path, body, includeAuth = true) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: buildHeaders(includeAuth),
    body: JSON.stringify(body),
  });
  return handleResponse(response);
}

async function apiPut(path, body) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "PUT",
    headers: buildHeaders(),
    body: JSON.stringify(body),
  });
  return handleResponse(response);
}

async function apiDelete(path) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "DELETE",
    headers: buildHeaders(),
  });
  return handleResponse(response);
}
