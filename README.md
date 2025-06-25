# 📡 CMSIS-DAP-Communicator 📡
Communicate through CMSIS-DAP with the PSoC6 / Onethinx OTX-18 Core module 

Firmware and Software for the **Onethinx Core OTX-18** module — an ultra-compact, production-ready LoRaWAN® module powered by PSoC6.

This firmware demonstrates memory communication, configuration provisioning, and runtime diagnostics using a CMSIS-DAP programmer. It includes direct USB HID communication support via a simple .NET utility.

---

## 🔧 Key Features

- Memory-mapped command structure (`CommData_t`) for easy interaction
- LoRaWAN® key management (OTAA/ABP, 10x/11x)
- Firmware build metadata and diagnostics
- LED control and sensor implementation
- Pre-configured LoRaWAN® stack support
- UART-based `printf()` logging using a lightweight drop-in (`PrintF.c/h`)
  
---

## 💻 PC-Side Communication Utility

![CMSIS-DAP Communicator](https://github.com/onethinx/Readme_assets/blob/main/CMSIS-DAP-Communicator.png?raw=true)

A .NET 8 desktop app is included to communicate with the module via **CMSIS-DAP over USB HID**. It allows you to:

- View and edit LoRaWAN keys
- Read sensor values (Light intensity throug ADC on the OTX-18 DevKit)
- Query LoRaWAN Stack and Firmware info
- Toggle I/O such as LEDs

---

## ⚡ Getting Started

### Firmware

1. Open the project in **Visual Studio Code** (check [OTX18-Project-Examples](https://github.com/onethinx/OTX18-Project-Examples) for more information)
2. Connect a CMSIS-DAP compatible programmer.
3. Build and flash the firmware.

### PC Utility


1. Open `CmsisDap_Communicator.sln` in **Visual Studio 2022+**
2. Build and run.
3. Connect to the OTX-18 module and start interacting!

---

## 🧩 PrintF.c/h

A compact UART `printf()` implementation is included as a drop-in. No extra dependencies, minimal footprint. Ideal for debugging embedded systems without wiring RX and TX to external terminals.

---

## 📄 License

[MIT License](https://github.com/onethinx/OTX-CMSIS-DAP-Communicator/blob/main/LICENSE)

---

## More Info

🚀 Visit [onethinx.com](https://onethinx.com) or contact us at [info@onethinx.com](mailto:info@onethinx.com) 🚀
