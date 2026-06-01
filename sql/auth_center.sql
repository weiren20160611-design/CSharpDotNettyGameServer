/*
 Navicat Premium Dump SQL

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 80405 (8.4.5)
 Source Host           : localhost:3306
 Source Schema         : auth_center

 Target Server Type    : MySQL
 Target Server Version : 80405 (8.4.5)
 File Encoding         : 65001

 Date: 01/06/2026 10:05:38
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account`  (
  `uid` bigint NOT NULL DEFAULT 0 COMMENT '玩家唯一的UID号',
  `unick` varchar(32) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '\"\"' COMMENT '玩家的昵称',
  `usex` int NOT NULL DEFAULT 0 COMMENT '0:男, 1:女的',
  `uface` int NOT NULL DEFAULT 0 COMMENT '系统默认图像，自定义图像后面再加上',
  `uname` varchar(32) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT '\"\"' COMMENT '玩家的账号名称',
  `upwd` varchar(32) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT '\"\"' COMMENT '玩家密码的MD5值',
  `phone` varchar(16) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT '\"\"' COMMENT '玩家的电话',
  `guest_key` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '0' COMMENT '游客账号的唯一的key',
  `email` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT '\"\"' COMMENT '玩家的email',
  `address` varchar(128) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT '\"\"' COMMENT '玩家的地址',
  `uvip` int NOT NULL DEFAULT 0 COMMENT '玩家VIP的等级，这个是最普通的',
  `vip_end_time` int NOT NULL DEFAULT 0 COMMENT '玩家VIP到期的时间撮',
  `is_guest` int NOT NULL DEFAULT 0 COMMENT '标志改账号是否为游客账号',
  `status` int NOT NULL DEFAULT 0 COMMENT '0正常，其他的根据需求来定',
  `uchannel` int NOT NULL DEFAULT 0 COMMENT '玩家注册对应的渠道，微信，抖音等',
  PRIMARY KEY (`uid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '存放我们的玩家信息' ROW_FORMAT = DYNAMIC;

SET FOREIGN_KEY_CHECKS = 1;
