import { describe, it, expect, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import { ThemeProvider, useTheme } from "../contexts/ThemeContext";

describe("ThemeContext", () => {
  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear();
    // Reset document class list
    document.documentElement.classList.remove("light", "dark");
  });

  it("provides default theme as system", () => {
    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    expect(result.current.theme).toBe("system");
  });

  it("applies light theme when set", async () => {
    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    act(() => {
      result.current.setTheme("light");
    });

    await waitFor(() => {
      expect(result.current.theme).toBe("light");
      expect(result.current.effectiveTheme).toBe("light");
      expect(document.documentElement.classList.contains("light")).toBe(true);
    });
  });

  it("applies dark theme when set", async () => {
    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    act(() => {
      result.current.setTheme("dark");
    });

    await waitFor(() => {
      expect(result.current.theme).toBe("dark");
      expect(result.current.effectiveTheme).toBe("dark");
      expect(document.documentElement.classList.contains("dark")).toBe(true);
    });
  });

  it("persists theme to localStorage", async () => {
    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    act(() => {
      result.current.setTheme("dark");
    });

    await waitFor(() => {
      expect(localStorage.getItem("theme")).toBe("dark");
    });
  });

  it("loads theme from localStorage on mount", () => {
    localStorage.setItem("theme", "dark");

    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    expect(result.current.theme).toBe("dark");
  });

  it("throws error when used outside provider", () => {
    // Suppress console.error for this test
    const originalError = console.error;
    console.error = () => {};

    expect(() => {
      renderHook(() => useTheme());
    }).toThrow("useTheme must be used within a ThemeProvider");

    console.error = originalError;
  });

  it("switches between themes correctly", async () => {
    const { result } = renderHook(() => useTheme(), {
      wrapper: ThemeProvider,
    });

    // Start with light
    act(() => {
      result.current.setTheme("light");
    });

    await waitFor(() => {
      expect(result.current.theme).toBe("light");
    });

    // Switch to dark
    act(() => {
      result.current.setTheme("dark");
    });

    await waitFor(() => {
      expect(result.current.theme).toBe("dark");
      expect(document.documentElement.classList.contains("dark")).toBe(true);
      expect(document.documentElement.classList.contains("light")).toBe(false);
    });

    // Switch to system
    act(() => {
      result.current.setTheme("system");
    });

    await waitFor(() => {
      expect(result.current.theme).toBe("system");
    });
  });
});
