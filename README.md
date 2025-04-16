# 动态表情包制作器 / Dynamic Sticker Maker

> 基于原创角色形象的粉丝向工具，可输出带透明底的 GIF 动图  
> A fan-oriented tool based on original character design, supporting transparent GIF output

---

## 项目简介 / Overview

该工具支持将多个表情组件组合、预览并导出为动态透明 GIF 图像。

This tool allows users to combine and preview multiple facial components, then export them as transparent animated GIFs. 

---

## 操作方式 / Controls

- 鼠标左键点击选中组件 / **Left Click** to select component
- 鼠标左键（按住）拖动组件 / **Left Mouse Button (Hold)** to drag
- 鼠标中键缩放组件 / **Middle Mouse Button** to scale
- 鼠标右键旋转组件 / **Right Mouse Button** to rotate
- 点击标签切换组件列表 / **Click tabs** to switch between component categories
- 列表悬停预览动画 / **Hover on list items** to preview animations

## 工具栏功能 / Toolbar Features

- 图层前置 / 后置，水平 / 垂直翻转组件  
  **Bring forward / send back**, **flip horizontally / vertically**
- 播放 / 暂停画布预览 / **Play / Pause canvas** to preview animation
- 一键删除选中组件 / **Delete selected component** with one click
- 自定义导出文件名与路径 / **Custom export**: set output filename and folder

---

## 技术细节 / Technical Details

- 使用 [`StandaloneFileBrowser`](https://github.com/gkngkc/UnityStandaloneFileBrowser) 实现文件夹/文件选择  
- 使用 [`ImageMagick`](https://imagemagick.org/) 将 PNG 序列合成为 GIF 动图  

---

## 示例 / Preview

![IMG_3183](https://github.com/user-attachments/assets/161f7592-c94a-4465-9027-fe10c76e9537)


---

