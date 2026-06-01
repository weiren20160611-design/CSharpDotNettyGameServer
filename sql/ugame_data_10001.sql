/*
 Navicat Premium Dump SQL

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 80405 (8.4.5)
 Source Host           : localhost:3306
 Source Schema         : ugame_data_10001

 Target Server Type    : MySQL
 Target Server Version : 80405 (8.4.5)
 File Encoding         : 65001

 Date: 01/06/2026 10:05:29
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for bonues
-- ----------------------------
DROP TABLE IF EXISTS `bonues`;
CREATE TABLE `bonues`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '领取奖励的唯一ID号',
  `uid` bigint NOT NULL COMMENT '用户的UID',
  `tid` int NOT NULL DEFAULT 0 COMMENT '奖励对应的类型',
  `status` int NOT NULL DEFAULT 0 COMMENT '是否已经领取, 0未领取，1已领取,2已过期',
  `time` bigint NOT NULL DEFAULT 0 COMMENT '发放奖励的时间戳',
  `endTime` bigint NOT NULL DEFAULT 0 COMMENT 'endTime: 奖励结束领取的时间;',
  `bonuesDesic` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '  ' COMMENT '发生奖励的原因',
  `bonues1` int NOT NULL DEFAULT 0 COMMENT '第1个奖励数据',
  `bonues2` int NOT NULL DEFAULT 0 COMMENT '第2个奖励数据',
  `bonues3` int NOT NULL DEFAULT 0 COMMENT '第3个奖励数据',
  `bonues4` int NOT NULL DEFAULT 0 COMMENT '第4个奖励数据',
  `bonues5` int NOT NULL DEFAULT 0 COMMENT '第5个奖励数据',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 19 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '玩家奖励管理' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for emailmessage
-- ----------------------------
DROP TABLE IF EXISTS `emailmessage`;
CREATE TABLE `emailmessage`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '邮件消息的唯一ID号',
  `fromPlayerId` bigint NOT NULL DEFAULT -1 COMMENT '来自与哪个玩家Id发送的邮件',
  `toPlayerId` bigint NOT NULL DEFAULT 0 COMMENT '发送给哪个玩家Id的邮件',
  `msgBody` varchar(256) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '' COMMENT '邮件消息主题内容',
  `status` int NOT NULL DEFAULT 0 COMMENT '当前邮件消息的状态，0，表示未读， 1表示已读，2表示已删除;',
  `sendTime` bigint NOT NULL DEFAULT 0 COMMENT '邮件消息发送的时间',
  `userData` bigint NOT NULL DEFAULT 0 COMMENT '消息邮件要关联的一些用户数据，具体可以根据游戏项目来定,暂定用BigInt,带任务ID，奖励ID等',
  `readTime` int NOT NULL DEFAULT 0 COMMENT '邮件消息被玩家阅读的时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '邮件消息管理' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for gamegoods
-- ----------------------------
DROP TABLE IF EXISTS `gamegoods`;
CREATE TABLE `gamegoods`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '物品实例ID号',
  `uid` bigint NOT NULL COMMENT '用户的UID',
  `tid` int NOT NULL DEFAULT 0 COMMENT '物品的类型ID，关联背包物品的配置表',
  `num` int NOT NULL DEFAULT 0 COMMENT '背包物品的数目',
  `strengData` varbinary(64) NULL DEFAULT 0x30 COMMENT '可选，不同游戏，可能不一样,保存当前装备的强化数据,可以考虑使用protobuf序列化',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 14 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '玩家当前的任务实例列表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for gametask
-- ----------------------------
DROP TABLE IF EXISTS `gametask`;
CREATE TABLE `gametask`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '任务实例ID号',
  `uid` bigint NOT NULL COMMENT '用户的UID',
  `tid` int NOT NULL DEFAULT 0 COMMENT '任务类型ID号',
  `status` int NOT NULL DEFAULT 0 COMMENT '当前任务的状态,0未开启,1进行中，2任务完成,3任务取消等',
  `startTime` bigint NOT NULL DEFAULT 0 COMMENT '开始任务的时间戳',
  `endTime` bigint NOT NULL DEFAULT 0 COMMENT '任务结束的时间戳，-1长期有效',
  `taskData` varbinary(64) NOT NULL DEFAULT 0x30 COMMENT '保存当前任务的数据进度，Protobuf编码',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '玩家当前的任务实例列表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for loginbonues
-- ----------------------------
DROP TABLE IF EXISTS `loginbonues`;
CREATE TABLE `loginbonues`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '领取奖励的唯一ID号',
  `uid` bigint NOT NULL COMMENT '用户的UID',
  `bonues` int NOT NULL DEFAULT 0 COMMENT '奖励的数目',
  `status` int NOT NULL DEFAULT 0 COMMENT '是否已经领取, 0未领取，1已领取',
  `bonues_time` bigint NOT NULL DEFAULT 0 COMMENT '发放奖励的时间',
  `days` int NOT NULL DEFAULT 0 COMMENT '连续登陆天数',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 57 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci COMMENT = '登陆奖励管理' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for player
-- ----------------------------
DROP TABLE IF EXISTS `player`;
CREATE TABLE `player`  (
  `id` bigint NOT NULL DEFAULT 10000 COMMENT 'playerId',
  `accountId` bigint NOT NULL DEFAULT 0 COMMENT '账号ID',
  `level` int NOT NULL DEFAULT 1 COMMENT '等级',
  `name` varchar(128) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT '用户名字',
  `usex` int NOT NULL DEFAULT 0,
  `job` int NOT NULL DEFAULT 0 COMMENT '职业',
  `exp` int NOT NULL DEFAULT 0 COMMENT '经验',
  `HP` int NOT NULL DEFAULT 0 COMMENT '玩家当前的HP',
  `MP` int NOT NULL DEFAULT 0 COMMENT '玩家当前的MP',
  `equipment` varchar(128) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT '装备情况',
  `ucoin` int NOT NULL DEFAULT 0 COMMENT '玩家的金币,游戏，奖励，卖装备获取, 购买消耗品',
  `umoney` int NOT NULL DEFAULT 0 COMMENT '玩家的元宝, 充值可以获得',
  `lastDailyReset` bigint NULL DEFAULT NULL,
  `vipRightJson` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT 'VIP特权',
  `platform` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT '用户在哪个平台',
  `status` int NOT NULL DEFAULT 0 COMMENT '账号的状态',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = DYNAMIC;

SET FOREIGN_KEY_CHECKS = 1;
