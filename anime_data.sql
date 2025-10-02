-- Insert genres first
INSERT INTO Genres (Name) VALUES
('Action'),
('Adventure'),
('Comedy'),
('Drama'),
('Fantasy'),
('Horror'),
('Romance'),
('Sci-Fi'),
('Supernatural'),
('Psychological'),
('Superhero'),
('Thriller'),
('Martial Arts'),
('School'),
('Mecha'),
('Slice of Life'),
('Mystery'),
('Historical'),
('Sports'),
('Isekai');

-- Insert anime
INSERT INTO Animes (Title, Synopsis, Year, Rating, Status, Episodes, Studio, CoverUrl, PosterUrl, Slug) VALUES
('Attack on Titan', 'Humanity fights against giant creatures in a post-apocalyptic world.', '2013-2023', 9.2, 'Completed', '87', 'MAPPA', '/images/attack_on_titan.jpg', '/images/attack_on_titan.jpg', 'attack-on-titan'),
('One Piece', 'Follow Luffy''s epic adventure to become the Pirate King.', '1999-Present', 9.1, 'Ongoing', '1000+', 'Toei Animation', '/images/one_piece.jpg', '/images/one_piece.jpg', 'one-piece'),
('Demon Slayer', 'A young boy''s journey to save his sister and defeat demons.', '2019-2022', 8.7, 'Completed', '44', 'Ufotable', '/images/demon_slayer.jpg', '/images/demon_slayer.jpg', 'demon-slayer'),
('My Hero Academia', 'Heroes and villains battle in a world where superpowers are common.', '2016-Present', 8.4, 'Ongoing', '113', 'Bones', '/images/my_hero_academia.jpg', '/images/my_hero_academia.jpg', 'my-hero-academia'),
('Naruto', 'A young ninja''s journey to become the strongest ninja and leader of his village.', '2002-2007', 8.3, 'Completed', '220', 'Pierrot', '/images/naruto.jpg', '/images/naruto.jpg', 'naruto'),
('Death Note', 'A student discovers a notebook that allows him to kill anyone by writing their name.', '2006-2007', 9.0, 'Completed', '37', 'Madhouse', '/images/death note.jpg', '/images/death note.jpg', 'death-note'),
('Dragon Ball', 'Goku''s adventures in a martial arts world.', '1986-1989', 8.7, 'Completed', '153', 'Toei Animation', '/images/dragon_ball.jpg', '/images/dragon_ball.jpg', 'dragon-ball'),
('Dragon Ball Z', 'Goku defends Earth from powerful villains.', '1989-1996', 8.8, 'Completed', '291', 'Toei Animation', '/images/dragon_ball_z.jpg', '/images/dragon_ball_z.jpg', 'dragon-ball-z'),
('Dragon Ball Super', 'Goku faces new challenges in a more powerful universe.', '2015-2018', 7.4, 'Completed', '131', 'Toei Animation', '/images/dragon_ball_super.jpg', '/images/dragon_ball_super.jpg', 'dragon-ball-super'),
('Boruto: Naruto Next Generations', 'The next generation of ninjas in the Naruto world.', '2017-Present', 6.9, 'Ongoing', '293', 'Pierrot', '/images/boruto.jpg', '/images/boruto.jpg', 'boruto-naruto-next-generations'),
('Kaiju No. 8', 'A young man fights giant monsters to protect humanity.', '2024-Present', 8.5, 'Ongoing', '12', 'Production I.G', '/images/kaiju_no_8.jpg', '/images/kaiju_no_8.jpg', 'kaiju-no-8'),
('Jujutsu Kaisen', 'Students at a magic school fight cursed spirits.', '2020-Present', 8.6, 'Ongoing', '24', 'MAPPA', '/images/jujutsu_kaisen.jpg', '/images/jujutsu_kaisen.jpg', 'jujutsu-kaisen'),
('Chainsaw Man', 'A devil hunter makes a contract with a chainsaw devil.', '2022-Present', 8.4, 'Ongoing', '12', 'MAPPA', '/images/chainsaw_man.jpg', '/images/chainsaw_man.jpg', 'chainsaw-man'),
('Attack on Titan: The Final Season', 'The conclusion of the Attack on Titan story.', '2020-2021', 9.1, 'Completed', '16', 'MAPPA', '/images/attack_on_titan_final.jpg', '/images/attack_on_titan_final.jpg', 'attack-on-titan-the-final-season'),
('Fullmetal Alchemist: Brotherhood', 'Two brothers search for the Philosopher''s Stone.', '2009-2010', 9.1, 'Completed', '64', 'Bones', '/images/full_metal_alchemist_brotherhood.jpg', '/images/full_metal_alchemist_brotherhood.jpg', 'fullmetal-alchemist-brotherhood'),
('Hunter x Hunter', 'A young boy''s journey to become a Hunter.', '2011-2014', 9.0, 'Completed', '148', 'Madhouse', '/images/hunter_x_hunter.jpg', '/images/hunter_x_hunter.jpg', 'hunter-x-hunter'),
('One Punch Man', 'A hero who can defeat any opponent with one punch.', '2015-Present', 8.7, 'Ongoing', '24', 'Madhouse', '/images/one_punch_man.jpg', '/images/one_punch_man.jpg', 'one-punch-man'),
('Tokyo Ghoul', 'A college student becomes a half-ghoul after an accident.', '2014', 7.8, 'Completed', '12', 'Pierrot', '/images/tokyo_ghoul.jpg', '/images/tokyo_ghoul.jpg', 'tokyo-ghoul'),
('Bleach', 'A Soul Reaper protects the living world from evil spirits.', '2004-2012', 8.1, 'Completed', '366', 'Pierrot', '/images/bleach.jpg', '/images/bleach.jpg', 'bleach'),
('Fairy Tail', 'A guild of wizards on magical adventures.', '2009-2019', 7.7, 'Completed', '328', 'A-1 Pictures', '/images/fairy_tail.jpg', '/images/fairy_tail.jpg', 'fairy-tail');

-- Insert anime-genre relationships
-- Attack on Titan (1): Action, Drama, Fantasy, Horror
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(1, 1), (1, 4), (1, 5), (1, 6);

-- One Piece (2): Action, Adventure, Comedy, Drama
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(2, 1), (2, 2), (2, 3), (2, 4);

-- Demon Slayer (3): Action, Supernatural, Drama
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(3, 1), (3, 9), (3, 4);

-- My Hero Academia (4): Action, Superhero, School
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(4, 1), (4, 11), (4, 14);

-- Naruto (5): Action, Adventure, Martial Arts
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(5, 1), (5, 2), (5, 13);

-- Death Note (6): Thriller, Psychological, Supernatural
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(6, 12), (6, 10), (6, 9);

-- Dragon Ball (7): Action, Adventure, Martial Arts, Comedy
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(7, 1), (7, 2), (7, 13), (7, 3);

-- Dragon Ball Z (8): Action, Adventure, Martial Arts, Superhero
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(8, 1), (8, 2), (8, 13), (8, 11);

-- Dragon Ball Super (9): Action, Adventure, Martial Arts, Superhero
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(9, 1), (9, 2), (9, 13), (9, 11);

-- Boruto (10): Action, Adventure, Martial Arts
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(10, 1), (10, 2), (10, 13);

-- Kaiju No. 8 (11): Action, Sci-Fi, Horror, Supernatural
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(11, 1), (11, 8), (11, 6), (11, 9);

-- Jujutsu Kaisen (12): Action, Supernatural, Drama, Horror
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(12, 1), (12, 9), (12, 4), (12, 6);

-- Chainsaw Man (13): Action, Supernatural, Horror, Comedy
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(13, 1), (13, 9), (13, 6), (13, 3);

-- Attack on Titan Final Season (14): Action, Drama, Fantasy, Horror
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(14, 1), (14, 4), (14, 5), (14, 6);

-- Fullmetal Alchemist Brotherhood (15): Action, Adventure, Drama, Fantasy
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(15, 1), (15, 2), (15, 4), (15, 5);

-- Hunter x Hunter (16): Action, Adventure, Fantasy, Martial Arts
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(16, 1), (16, 2), (16, 5), (16, 13);

-- One Punch Man (17): Action, Comedy, Superhero, Sci-Fi
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(17, 1), (17, 3), (17, 11), (17, 8);

-- Tokyo Ghoul (18): Action, Horror, Supernatural, Drama
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(18, 1), (18, 6), (18, 9), (18, 4);

-- Bleach (19): Action, Adventure, Supernatural, Comedy
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(19, 1), (19, 2), (19, 9), (19, 3);

-- Fairy Tail (20): Action, Adventure, Comedy, Fantasy
INSERT INTO AnimeGenres (AnimeId, GenreId) VALUES
(20, 1), (20, 2), (20, 3), (20, 5);