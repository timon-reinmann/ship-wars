// @ts-check
const { test, expect } = require("@playwright/test");

test("has title", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/index.html");

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/ShipWars/);
});

test("get started link", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/index.html");

  // Click the get started link.
  await page.getByRole("link", { name: "VS Player" }).click();

  // Expects page to have a heading with the name of Installation.
  await expect(page.getByText(/Home/)).toBeVisible();

  await page.click("#lobbyinput");

  await page.getByRole("button", { name: "Link for friend" }).click();

  await page.click("#button--copy");

  await expect(page.getByText(/Join Game/)).toBeVisible();
});
