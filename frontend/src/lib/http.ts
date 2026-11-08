import axios from "axios";
import { env } from "@/config/env";

export const http = axios.create({
  baseURL: env.apiBaseUrl,
  headers: {
    "Content-Type": "application/json",
  },
});

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data;
    if (typeof data === "string" && data.trim()) {
      return data;
    }
    if (data && typeof data === "object") {
      if ("detail" in data && typeof data.detail === "string") {
        return data.detail;
      }
      if ("title" in data && typeof data.title === "string") {
        return data.title;
      }
      if ("message" in data && typeof data.message === "string") {
        return data.message;
      }
      if ("errors" in data && data.errors && typeof data.errors === "object") {
        const first = Object.values(data.errors)[0];
        if (Array.isArray(first) && first.length > 0) {
          return String(first[0]);
        }
      }
    }
    return error.message || "An unexpected error occurred.";
  }
  if (error instanceof Error) {
    return error.message;
  }
  return "An unexpected error occurred.";
}
