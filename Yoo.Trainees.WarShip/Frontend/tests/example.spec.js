// @ts-check
const { test, expect } = require("@playwright/test");
const { text } = require("stream/consumers");

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

  await page.waitForLoadState("networkidle");

  await page.getByRole("button", { name: "Link for friend" }).click();

  await page.click("#button--copy");

  await expect(page.getByText(/Game/)).toBeVisible();

  await page.getByText(/Game/).click();

  await expect(page.getByText(/Commit/)).toBeVisible();
});

test("get startet email", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/html/invite.html");

  await page.click("#lobbyinput");

  await page.waitForLoadState("networkidle");

  await page.getByRole("button", { name: "Link for friend" }).click();

  await page.getByRole("link", { name: "send Email" }).click();

  await page.fill("#email-input", "alex@uscata.com");

  await page.click("#email");

  await page.getByText(/Game/).click();
});

test("place ships", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/html/invite.html");

  await page.click("#lobbyinput");

  await page.waitForLoadState("networkidle");

  await page.getByRole("button", { name: "Link for friend" }).click();

  await page.click("#button--copy");

  await page.getByText(/Game/).click();

  await page.locator("#warship1").dragTo(page.locator("#box0"));

  await page.locator("#cruiser1").dragTo(page.locator("#box20"));

  await page.locator("#cruiser2").dragTo(page.locator("#box40"));

  await page.locator("#destroyer1").dragTo(page.locator("#box60"));

  await page.locator("#destroyer2").dragTo(page.locator("#box80"));

  await page.locator("#destroyer3").dragTo(page.locator("#box6"));

  await page.locator("#submarine1").dragTo(page.locator("#box77"));

  await page.locator("#submarine2").dragTo(page.locator("#box79"));

  await page.locator("#submarine3").dragTo(page.locator("#box97"));

  await page.locator("#submarine4").dragTo(page.locator("#box99"));

  await page.click("#commit_button");

  await page.waitForLoadState("networkidle");

  await expect(page.getByText(/finished!!!/)).toBeVisible();
});

test("send message", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/html/invite.html");

  await page.click("#lobbyinput");

  await page.waitForLoadState("networkidle");

  await page.getByRole("button", { name: "Link for friend" }).click();

  await page.click("#button--copy");

  await page.getByText(/Game/).click();

  await page.click("#messageInput");

  await page.fill("#messageInput", "Hello");

  await page.click("#sendButton");

  await expect(page.getByText(/Hans:/)).toBeVisible();
});

test("open directions", async ({ page }) => {
  await page.goto("http://127.0.0.1:5500/Frontend/index.html");

  await page.click("#sidebarBtn");

  await expect(
    page.getByText(/Click on the topic that interests you./)
  ).toBeVisible();
});
