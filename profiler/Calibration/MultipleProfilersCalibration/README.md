# MultipleProfilerCalibration Sample

With this sample, you can calibrate multiple profilers that simultaneously scan the same target object, and output the calibration results and errors, stitching results, the stitched depth map, and the stitched point cloud.

If you have any questions or have anything to share, feel free to post on the [Mech-Mind Online Community](https://community.mech-mind.com/). The community also contains a [specific category for development with Mech-Eye SDK](https://community.mech-mind.com/c/mech-eye-sdk-development/19).

## Sample Usage

### Overview

This sample program performs calibration and stitching operations for multiple laser profilers that scan the same object. It supports:

1. Calibration of laser profilers relative to a primary device.
2. Point cloud stitching in the scenario of one primary device.
3. Point cloud stitching in the scenario of multiple primary devices.

---

### 1. Main Menu Operations

Launch the program and choose an operation:

| Command | Description                                                        |
| ------- | ------------------------------------------------------------------ |
| `C`     | Calibrate secondary profilers relative to a primary laser profiler |
| `S`     | Stitch point clouds using one primary laser profiler               |
| `M`     | Stitch point clouds using multiple primary laser profilers         |

---

### 2. Calibration Workflow (Option: `C`)

#### Step 1: Define Frustum Dimensions

Input the dimensions of the frustums of the calibration target when prompted:

* **Top Length** (mm): Upper base length of the frustums (e.g., `100.0`).
* **Bottom Length** (mm): Lower base length of the frustums (e.g., `150.0`).
* **Height** (mm): Height of the frustums (e.g., `50.0`).

> 💡 *Confirm values with `y` or re-enter if incorrect.*

#### Step 2: Connect Profilers

* Automatic detection connects all available profilers.
* Ensure devices are powered and accessible via SDK.

#### Step 3: Identify Devices

* **Major Profiler**: Primary reference device (usually first in the device list)
* **Minor Profilers**: Secondary devices to calibrate relative to the primary device.

#### Step 4: Set Calibration Parameters

For each secondary laser profiler, configure:

**Mode Selection:**

* `wide` (translation-based, i.e., side-by-side or in the reversed direction)
* `angle` (rotation-based, i.e., in the opposite direction or around a circle)

**Pose Parameters:**

* *Translation Mode*:
  * Distance (mm) between two frustums of the calibration target (e.g., `200.0`).
  * Translation axis (`X`, `Y`, or `Z`): the axis along which one frustum is translated relative to another.
* *Rotation Mode*:
  * Rotation angle (°) between two frustums of the calibration target (e.g., `30.0`).
  * Rotation radius (mm): distance from the rotation center to the midpoint of the frustum's height (e.g., `50.0`).
  * Rotation axis (`X`, `Y`, or `Z`): the axis along which one frustum is rotated relative to another.

> 💡 *Review inputs and confirm with `y`.*

#### Step 5: Execute Calibration

* Data acquisition is initiated automatically.
* **Ensure**:
  * A clear view of the calibration target
  * Stable ambient lighting conditions
* Results are saved automatically to the specified folder (e.g., `C:/calibration_results`)

#### Step 6: Stitching (Optional)

* After calibration, enter `y` to start stitching and/or fusing:
  * Stitched point cloud is saved as a `.ply` file.
  * Perform fusion if the laser profilers are Z-parallel and not in the opposite setup.

---

### 3. Single Primary Stitching (Option: `S`)

#### Step 1: Connect Laser Profilers

* Devices are automatically detected.

#### Step 2: Acquire Data

* System acquires 2D images and depth maps from all profilers.

#### Step 3: Load Calibration Data

* Provide path to existing calibration files (e.g., `C:/calibration_files`)

#### Step 4: Stitch and Save

* Stitched point cloud is generated and saved at specified path.
* Follow on-screen prompts for fusion (if applicable).

---

### 4. Multi-Primary Stitching (Option: `M`)

#### Step 1: Load Multiple Calibration Files

Input paths for three calibration pairs:

1. Primary 0 + Secondary 1 (e.g., `C:/calib_01`)
2. Primary 0 + Secondary 2 (e.g., `C:/calib_02`)
3. Primary 2 + Secondary 3 (e.g., `C:/calib_23`)

#### Step 2: Connect Profilers

* System verifies connection for all laser profilers.

#### Step 3: Acquire Data

* Data is automatically acquired from all laser profilers.

#### Step 4: Stitch Across Devices

* Computes transformations using loaded calibration data.
* Saves stitched point cloud to user-defined path.

---

### Key Notes

#### File Paths

* Use **English-only characters** in paths (e.g., avoid characters in `中文` or `日本語`)
* Ensure write permissions to target folders

#### Profiler Arrangement

* Z-parallel setups yield best fusion results.
* Opposite setups may prevent fusion.

#### Error Handling

* Detailed error messages are displayed for failed data acquisition or calibrations.
* Check device connections and retry.

### Troubleshooting Tips

| Error Message          | Solution                                         |
| ---------------------- | ------------------------------------------------ |
| "Capture failed"       | Ensure all laser profilers are visible to system |
| "Failed to save files" | Verify folder permissions and path validity      |
| Calibration errors     | Re-measure target dimensions and retry           |

