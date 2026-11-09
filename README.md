\# Flow Governance Manager



An \[XrmToolBox](https://www.xrmtoolbox.com/) plugin for identifying and remediating unhealthy Power Automate flows at scale in Microsoft Dataverse environments.



!\[Flow Governance Manager Screenshot](docs/screenshots/main-view.png)



\## Overview



Managing Power Automate flows across a Dataverse environment is difficult when you have dozens or hundreds of flows owned by different users. Flows get turned off, owners leave the organization, runs fail silently, and nobody notices until a business process breaks.



Flow Governance Manager gives you a single view of every flow in your environment with a health score for each one, so you can find and fix problems before they become incidents.



\## Features



\- \*\*Health Scoring\*\* — Every flow gets a score from 0 to 100 based on run history, error rate, flow status, and owner activity

\- \*\*Color Coded Grid\*\* — Green, yellow, and red rows let you spot critical flows at a glance

\- \*\*Statistics Panel\*\* — See total flows, healthy/at-risk/critical counts, ownerless flows, and flows inactive for 90+ days

\- \*\*Filter by Health Status\*\* — Quickly isolate Critical or At Risk flows

\- \*\*Filter by Flow Status\*\* — View active-only or inactive-only flows

\- \*\*Export to Excel\*\* — Export the full governance report to a formatted .xlsx file

\- \*\*Solution Aware\*\* — Shows which solution each flow belongs to



\## Health Scoring



Each flow starts at 100 and points are deducted based on governance risk factors:



| Risk Factor | Deduction |

|---|---|

| Owner is disabled / left organization | -40 |

| Flow has never successfully run | -40 |

| Error rate above 50% | -35 |

| Flow is turned off | -20 |

| Error rate above 20% | -20 |

| No successful run in 90+ days | -20 |

| No successful run in 30-90 days | -10 |



\*\*Health Status thresholds:\*\*



| Score | Status |

|---|---|

| 70 and above | Healthy |

| 40 to 69 | At Risk |

| Below 40 | Critical |



\## Screenshots



\### Main View

!\[Main View](docs/screenshots/main-view.png)



\### Filter by Health Status

!\[Filter Example](docs/screenshots/filter-view.png)



\### Export to Excel

!\[Excel Export](docs/screenshots/excel-export.png)



\## Installation



\### Via XrmToolBox Plugin Store (recommended)

1\. Open XrmToolBox

2\. Go to \*\*Tools\*\* > \*\*Plugin Store\*\*

3\. Search for \*\*Flow Governance Manager\*\*

4\. Click \*\*Install\*\*

5\. Restart XrmToolBox



\### Manual Installation

1\. Download the latest `.nupkg` from the \[Releases](https://github.com/knkumar-1215/XrmToolBox-FlowGovernanceManager/releases) page

2\. Copy the extracted DLL into your XrmToolBox `Plugins` folder

3\. Restart XrmToolBox



\## Usage



1\. Connect XrmToolBox to your Dataverse environment

2\. Open \*\*Flow Governance Manager\*\* from the tool list

3\. Click \*\*Load Flows\*\*

4\. Review the health scores and color coded grid

5\. Use the \*\*Health Status\*\* and \*\*Flow Status\*\* dropdowns to filter

6\. Click \*\*Export to Excel\*\* to save a governance report



\## Requirements



\- XrmToolBox 1.2023.x or later

\- .NET Framework 4.6.2

\- Dataverse environment with Power Automate flows



\## Permissions Required



The plugin reads the following Dataverse tables:



\- `workflow` — Flow definitions and status

\- `systemuser` — Owner details and active status

\- `flowrun` — Run history and error rates

\- `solution` / `solutioncomponent` — Solution membership



Read-only access is sufficient. No data is written to Dataverse.



\## Building from Source



```bash

git clone https://github.com/knkumar-1215/XrmToolBox-FlowGovernanceManager.git

cd XrmToolBox-FlowGovernanceManager

```



Open `FlowGovernanceManager.sln` in Visual Studio 2022 and build in Release mode.



\*\*Dependencies (via NuGet):\*\*

\- XrmToolBox (latest)

\- Microsoft.CrmSdk.CoreAssemblies

\- MiniExcel



\## Roadmap



\*\*V2 planned features:\*\*

\- Bulk ownership reassignment — reassign multiple flows to a new owner in one action

\- Environment comparison — compare flow health across DEV, UAT, and PROD side by side

\- Deprecated connector detection — flag flows using retired or soon-to-retire connectors

\- Configurable scoring thresholds — adjust deduction weights via settings form

\- Flow dependency map — show which flows trigger other flows so you understand blast radius before making changes



\*\*V3 planned features:\*\*

\- AI-assisted flow analysis — get plain English explanation of why a flow is unhealthy and what to do about it

\- Natural language search — query flows using plain English ("show me all flows owned by disabled users in production")

\- Predictive health alerts — identify flows trending toward failure before they break

\- Automated governance reports — AI-generated narrative summary of environment health sent on a schedule

\- Governance policy engine — define org-wide rules and automatically flag violations



\## Author



\*\*Nagendra kumar Kosaraju\*\*

Senior Dataverse and Power Platform Developer



\[GitHub](https://github.com/knkumar-1215)



\## License



MIT License. See \[LICENSE](LICENSE) for details.



\## Contributing



Issues and pull requests are welcome. Please open an issue first to discuss any significant changes.

