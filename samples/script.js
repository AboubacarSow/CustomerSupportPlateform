const language = navigator.language
    .toLowerCase()
    .startsWith("tr")
    ? "tr"
    : "en";

const content = language === "tr"
    ? tr_content
    : en_content;


let currentLanguage =
    localStorage.getItem("systek-language") ||
    (navigator.language.toLowerCase().startsWith("tr") ? "tr" : "en");

function getContent() {
    return currentLanguage === "tr"
        ? tr_content
        : en_content;
}

function setLanguage(language) {
    currentLanguage = language;

    localStorage.setItem("systek-language", language);

    applyContent();

    document.documentElement.lang = language;
}


function applyContent() {
    var content = getContent()
    
    // Navigation
    document.getElementById("nav-services").textContent =
        content.nav.services;

    document.getElementById("nav-support").textContent =
        content.nav.support;

    document.getElementById("nav-integrate").textContent =
        content.nav.integrate;

    document.getElementById("nav-cta").textContent =
        content.nav.cta;


    // Hero

    document.getElementById("hero-eyebrow").textContent =
        content.hero.eyebrow;

    document.getElementById("hero-title").innerHTML =
        content.hero.title;

    document.getElementById("hero-description").textContent =
        content.hero.description;

    document.getElementById("hero-primary").textContent =
        content.hero.primaryButton;

    document.getElementById("hero-secondary").textContent =
        content.hero.secondaryButton;


    // Trust

    document.getElementById("trust-item-1").innerHTML =
        content.trust.item1;

    document.getElementById("trust-item-2").innerHTML =
        content.trust.item2;

    document.getElementById("trust-item-3").innerHTML =
        content.trust.item3;

    document.getElementById("trust-item-4").innerHTML =
        content.trust.item4;


    // Services

    document.getElementById("services-kicker").textContent =
        content.services.kicker;

    document.getElementById("services-title").textContent =
        content.services.title;

    document.getElementById("services-description").textContent =
        content.services.description;


    document.getElementById("service-digital-title").textContent =
        content.services.digitalTransformation.title;

    document.getElementById("service-digital-description").textContent =
        content.services.digitalTransformation.description;


    document.getElementById("service-management-title").textContent =
        content.services.managementConsulting.title;

    document.getElementById("service-management-description").textContent =
        content.services.managementConsulting.description;


    document.getElementById("service-data-title").textContent =
        content.services.dataKnowledge.title;

    document.getElementById("service-data-description").textContent =
        content.services.dataKnowledge.description;


    document.getElementById("service-ai-title").textContent =
        content.services.ai.title;

    document.getElementById("service-ai-description").textContent =
        content.services.ai.description;


    document.getElementById("service-enterprise-title").textContent =
        content.services.enterpriseSystems.title;

    document.getElementById("service-enterprise-description").textContent =
        content.services.enterpriseSystems.description;


    document.getElementById("service-org-title").textContent =
        content.services.organizationalDevelopment.title;

    document.getElementById("service-org-description").textContent =
        content.services.organizationalDevelopment.description;


    // Integration

    document.getElementById("integrate-kicker").textContent =
        content.integrate.kicker;

    document.getElementById("integrate-title").textContent =
        content.integrate.title;

    document.getElementById("integrate-description").textContent =
        content.integrate.description;


    const featuresElement =
        document.getElementById("integrate-features");

    content.integrate.features.forEach(feature => {
        const li = document.createElement("li");
        li.textContent = feature;
        featuresElement.appendChild(li);
    });


    // Footer

    document.getElementById("footer").textContent =
        content.footer;
 // HTML language
    document.documentElement.lang = currentLanguage;


    // Active language
    updateLanguageSwitcher();
}

function updateLanguageSwitcher() {
    const enButton = document.getElementById("lang-en");
    const trButton = document.getElementById("lang-tr");

    enButton.classList.toggle(
        "active",
        currentLanguage === "en"
    );

    trButton.classList.toggle(
        "active",
        currentLanguage === "tr"
    );
}


// Language events

document.getElementById("lang-en")
    .addEventListener("click", () => {
        setLanguage("en");
    });

document.getElementById("lang-tr")
    .addEventListener("click", () => {
        setLanguage("tr");
    });


// Initial render

applyContent();
