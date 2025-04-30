const express = require('express');
const mongoose = require('mongoose');
const app = express();

app.use(express.json());
// 允許跨域請求
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Content-Type');
    next();
});

// 連接MongoDB
mongoose.connect('mongodb://localhost:27017/pinball', {
    useNewUrlParser: true,
    useUnifiedTopology: true
});

// 玩家分數模型
const PlayerSchema = new mongoose.Schema({
    playerId: String,
    currentScore: { type: Number, default: 0 },
    bestScore: { type: Number, default: 0 }
});

const Player = mongoose.model('Player', PlayerSchema);

// 更新分數
app.post('/updateScore', async (req, res) => {
    try {
        const { playerId, currentScore, bestScore } = req.body;

        // 查找並更新玩家數據
        const result = await Player.findOneAndUpdate(
            { playerId },
            {
                currentScore,
                $max: { bestScore } // 只有當新分數更高時才更新最高分
            },
            { new: true, upsert: true }
        );

        res.json(result);
    } catch (error) {
        console.error('更新錯誤:', error);
        res.status(500).json({ error: error.message });
    }
});

// 獲取玩家數據
app.get('/player/:playerId', async (req, res) => {
    try {
        const player = await Player.findOne({ playerId: req.params.playerId });
        if (!player) {
            // 如果玩家不存在，創建新玩家
            const newPlayer = new Player({
                playerId: req.params.playerId,
                currentScore: 0,
                bestScore: 0
            });
            await newPlayer.save();
            return res.json(newPlayer);
        }
        res.json(player);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

const PORT = 3000;
app.listen(PORT, () => {
    console.log(`服務器運行在端口 ${PORT}`);
});